extern "C" {
	__constant__ float ToRad = 0.017453292519943295769236907684886;
	__constant__ float ToDeg = 57.295779513082320876798154814105;
	__constant__ int CountOfTrns = 512;
	__constant__ float3 trPoint1[512];
	__constant__ float3 trPoint2[512];
	__constant__ float3 trPoint3[512];
	__constant__ float3 normals[512];
	__constant__ float dRatios[512];
	__constant__ float3 colors[512];
	__global__ void resultPixel(const int2 resolution, const float3 xyz, const float3 light, const float fow, const float2 view, float3* outColors) {
		int row = blockIdx.y * blockDim.y + threadIdx.y;
		int col = blockIdx.x * blockDim.x + threadIdx.x;
		int i = col + row * resolution.x;
		float3 y;
		y.x = 255;
		y.y = 255;
		y.z = 255;
		outColors[i] = y;
		if (col >= resolution.x || row >= resolution.y) {
			return;
		}
		float3 dir;
		dir.y = col;
		dir.z = row;
		int2 halfRes;
		halfRes.x = resolution.x / 2;
		halfRes.y = resolution.y / 2;
		dir.y -= halfRes.x;
		dir.z -= halfRes.y;
		dir.z *= -1;
		dir.y /= halfRes.x;
		dir.z /= halfRes.y;
		float halfLenHor = sinf(fow * ToRad / 2);
		dir.y *= halfLenHor;
		dir.x = sqrt(1 - powf(dir.y, 2));
		float halfLenVer = halfLenHor / resolution.x * resolution.y;
		dir.z *= halfLenVer;
		float verRatio = sqrt(1 - pow(dir.z, 2));
		if (view.y != 0) {
			float2 temp;
			temp.y = verRatio * cos(view.y * ToRad) - dir.z * sin(view.y * ToRad);
			temp.x = verRatio * sin(view.y * ToRad) + dir.z * cos(view.y * ToRad);
			dir.z = temp.x;
			verRatio = temp.y;
		}
		dir.x *= verRatio;
		dir.y *= verRatio;
		if (view.x != 0) {
			float2 temp;
			temp.x = dir.x * cos(view.x * ToRad) - dir.y * sin(view.x * ToRad);
			temp.y = dir.x * sin(view.x * ToRad) + dir.y * cos(view.x * ToRad);
			dir.x = temp.x;
			dir.y = temp.y;
		}
		float3 baseColor;
		baseColor.x = 16;
		baseColor.y = 16;
		baseColor.z = 16;
		float minDist = 10000000;
		float3 out;
		out = baseColor;
		float3 newPoint;
		int skip;
		bool coled = false;
		float colRatio = 1;
		float2 pts[4];
		for (int j = 0; j < CountOfTrns; j++)
		{
			float3 abc = normals[j];
			float upper = dRatios[j] + abc.x * xyz.x + abc.y * xyz.y + abc.z * xyz.z;
			float lower = abc.x * dir.x + abc.y * dir.y + abc.z * dir.z;
			if (lower == 0 && upper == 0) {
				continue;
			}
			if (upper > 0 && lower == 0) {
				continue;
			}
			float t = -(upper / lower);
			if (t < 0) {
				continue;
			}
			newPoint.x = dir.x * t + xyz.x;
			newPoint.y = dir.y * t + xyz.y;
			newPoint.z = dir.z * t + xyz.z;
			if (abc.x != 0) {
				pts[0].x = newPoint.y;
				pts[0].y = newPoint.z;
				pts[1].x = trPoint1[j].y;
				pts[1].y = trPoint1[j].z;
				pts[2].x = trPoint2[j].y;
				pts[2].y = trPoint2[j].z;
				pts[3].x = trPoint3[j].y;
				pts[3].y = trPoint3[j].z;
			}
			else if (abc.y != 0) {
				pts[0].x = newPoint.x;
				pts[0].y = newPoint.z;
				pts[1].x = trPoint1[j].x;
				pts[1].y = trPoint1[j].z;
				pts[2].x = trPoint2[j].x;
				pts[2].y = trPoint2[j].z;
				pts[3].x = trPoint3[j].x;
				pts[3].y = trPoint3[j].z;
			}
			else {
				pts[0].x = newPoint.x;
				pts[0].y = newPoint.y;
				pts[1].x = trPoint1[j].x;
				pts[1].y = trPoint1[j].y;
				pts[2].x = trPoint2[j].x;
				pts[2].y = trPoint2[j].y;
				pts[3].x = trPoint3[j].x;
				pts[3].y = trPoint3[j].y;
			}
			float a;
			a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
			float b;
			b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
			float c;
			c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
			bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
			if (isIn) {

				float newLen = sqrt(pow(newPoint.x - xyz.x, 2) + pow(newPoint.y - xyz.y, 2) + pow(newPoint.z - xyz.z, 2));
				bool around = newLen < minDist;
				if (around) {
					minDist = newLen;
					colRatio = (light.x - newPoint.x) * abc.x
						+ (light.y - newPoint.y) * abc.y
						+ (light.z - newPoint.z) * abc.z;
					if (colRatio < 0)
						colRatio = 0;
					out = colors[j];
					skip = j;
					coled = true;
				}
			}
		}
		bool shadow = false;
		if (coled) {
			float3 pos = newPoint;
			float3 dir;
			dir.x = light.x - pos.x;
			dir.y = light.y - pos.y;
			dir.z = light.z - pos.z;
			float dirLen = norm3df(dir.x, dir.y, dir.z);
			dir.x /= abs(dirLen);
			dir.y /= abs(dirLen);
			dir.z /= abs(dirLen);
			for (int id = 0; id < CountOfTrns; id++) {
				if (id == skip)
					continue;
				float3 nor = normals[id];
				//float upper = dRatios[j] + abc.x * xyz.x + abc.y * xyz.y + abc.z * xyz.z;
				//float lower = abc.x * dir.x + abc.y * dir.y + abc.z * dir.z;
				float u = dRatios[id] + nor.x * pos.x + nor.y * pos.y + nor.z * pos.z;
				float l = nor.x * dir.x + nor.y * dir.y + nor.z * dir.z;
				if (l == 0 && u >= 0)
					continue;
				float t = -(u / l);
				if (t < 0)
					continue;
				float3 sdPt;
				sdPt.x = dir.x * t + pos.x;
				sdPt.y = dir.y * t + pos.y;
				sdPt.z = dir.z * t + pos.z;
				if (nor.x != 0) {
					pts[0].x = sdPt.y;
					pts[0].y = sdPt.z;
					pts[1].x = trPoint1[id].y;
					pts[1].y = trPoint1[id].z;
					pts[2].x = trPoint2[id].y;
					pts[2].y = trPoint2[id].z;
					pts[3].x = trPoint3[id].y;
					pts[3].y = trPoint3[id].z;
				}
				else if (nor.y != 0) {
					pts[0].x = sdPt.x;
					pts[0].y = sdPt.z;
					pts[1].x = trPoint1[id].x;
					pts[1].y = trPoint1[id].z;
					pts[2].x = trPoint2[id].x;
					pts[2].y = trPoint2[id].z;
					pts[3].x = trPoint3[id].x;
					pts[3].y = trPoint3[id].z;
				}
				else {
					pts[0].x = sdPt.x;
					pts[0].y = sdPt.y;
					pts[1].x = trPoint1[id].x;
					pts[1].y = trPoint1[id].y;
					pts[2].x = trPoint2[id].x;
					pts[2].y = trPoint2[id].y;
					pts[3].x = trPoint3[id].x;
					pts[3].y = trPoint3[id].y;
				}
				float a;
				a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
				float b;
				b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
				float c;
				c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
				bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
				if (isIn) {
					shadow = true;
					break;
				}

			}
		}
		if (shadow) {
			colRatio = 1;
			out = y;
		}
		colRatio = pow(abs(colRatio), 0.3);
		out.x *= colRatio;
		out.y *= colRatio;
		out.z *= colRatio;
		outColors[i] = out;

	}
}
