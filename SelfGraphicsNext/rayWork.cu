extern "C" {
	typedef struct {
		int objId;
		float3 p1;
		float3 p2;
		float3 p3;
		float3 nor;
		float3 col;
		float d;
	} PolygonCUDA;
	__constant__ float ToRad = 0.017453292519943295769236907684886;
	__constant__ float ToDeg = 57.295779513082320876798154814105;
	__global__ void resultPixel(const PolygonCUDA* pgs, const int polCount,
		const int2 resolution, const float3 xyz,
		const float3 light, const float fow,
		const float2 view, float3* outColors) {
		//Getting position of thread
		int row = blockIdx.y * blockDim.y + threadIdx.y;
		int col = blockIdx.x * blockDim.x + threadIdx.x;
		int i = col + row * resolution.x;
		if (col >= resolution.x || row >= resolution.y) {
			return;
		}
		//Init variables
		float3 dir;
		float3 baseColor;
		float3 out;
		float3 newPoint;
		float3 colPoint;
		float3 abc;
		float2 temp;
		float2 pts[4];
		float2 tg;
		int2 halfRes;
		float minDist = -1;
		float colRatio = 1;
		float upper;
		float lower;
		float a, b, c;
		bool coled = false;
		int skip;
		PolygonCUDA pol;
		float radValue;

		//Setting values
		baseColor.x = 16;
		baseColor.y = 16;
		baseColor.z = 16;
		out = baseColor;

		//Getting current thead directon
		dir.y = col;
		dir.z = row;
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
			radValue = view.y * ToRad;
			tg.x = cos(radValue);
			tg.y = sin(radValue);
			temp.y = verRatio * tg.x - dir.z * tg.y;
			temp.x = verRatio * tg.y + dir.z * tg.x;
			dir.z = temp.x;
			verRatio = temp.y;
		}
		dir.x *= verRatio;
		dir.y *= verRatio;
		if (view.x != 0) {
			radValue = view.x * ToRad;
			tg.x = cos(radValue);
			tg.y = sin(radValue);
			temp.x = dir.x * tg.x - dir.y * tg.y;
			temp.y = dir.x * tg.y + dir.y * tg.x;
			dir.x = temp.x;
			dir.y = temp.y;
		}

		//Counting first colision point
		for (int j = 0; j < polCount; j++)
		{
			pol = pgs[j];
			abc = pol.nor;
			upper = pol.d + abc.x * xyz.x + abc.y * xyz.y + abc.z * xyz.z;
			lower = abc.x * dir.x + abc.y * dir.y + abc.z * dir.z;
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
				pts[1].x = pol.p1.y;
				pts[1].y = pol.p1.z;
				pts[2].x = pol.p2.y;
				pts[2].y = pol.p2.z;
				pts[3].x = pol.p3.y;
				pts[3].y = pol.p3.z;
			}
			else if (abc.y != 0) {
				pts[0].x = newPoint.x;
				pts[0].y = newPoint.z;
				pts[1].x = pol.p1.x;
				pts[1].y = pol.p1.z;
				pts[2].x = pol.p2.x;
				pts[2].y = pol.p2.z;
				pts[3].x = pol.p3.x;
				pts[3].y = pol.p3.z;
			}
			else {
				pts[0].x = newPoint.x;
				pts[0].y = newPoint.y;
				pts[1].x = pol.p1.x;
				pts[1].y = pol.p1.y;
				pts[2].x = pol.p2.x;
				pts[2].y = pol.p2.y;
				pts[3].x = pol.p3.x;
				pts[3].y = pol.p3.y;
			}
			a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
			b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
			c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
			bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
			if (isIn) {

				float newLen = sqrt(pow(newPoint.x - xyz.x, 2) + pow(newPoint.y - xyz.y, 2) + pow(newPoint.z - xyz.z, 2));
				bool around;
				if (minDist == -1) {
					minDist = newLen;
					around = true;
				}
				else {
					around = newLen < minDist;
				}
				if (around) {
					minDist = newLen;
					colRatio = (light.x - newPoint.x) * abc.x
						+ (light.y - newPoint.y) * abc.y
						+ (light.z - newPoint.z) * abc.z;
					if (colRatio < 0)
						colRatio = 0;
					out = pol.col;
					skip = j;
					coled = true;
					colPoint = newPoint;
				}
			}
		}
		bool shadow = false;
		if (coled) {
			float3 pos = colPoint;
			dir.x = light.x - pos.x;
			dir.y = light.y - pos.y;
			dir.z = light.z - pos.z;
			float dirLen = norm3df(dir.x, dir.y, dir.z);
			dir.x /= dirLen;
			dir.y /= dirLen;
			dir.z /= dirLen;
			for (int id = 0; id < polCount; id++) {
				if (id == skip)
					continue;
				pol = pgs[id];
				float3 nor = pol.nor;
				float u = pol.d + nor.x * pos.x + nor.y * pos.y + nor.z * pos.z;
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
					pts[1].x = pol.p1.y;
					pts[1].y = pol.p1.z;
					pts[2].x = pol.p2.y;
					pts[2].y = pol.p2.z;
					pts[3].x = pol.p3.y;
					pts[3].y = pol.p3.z;
				}
				else if (nor.y != 0) {
					pts[0].x = sdPt.x;
					pts[0].y = sdPt.z;
					pts[1].x = pol.p1.x;
					pts[1].y = pol.p1.z;
					pts[2].x = pol.p2.x;
					pts[2].y = pol.p2.z;
					pts[3].x = pol.p3.x;
					pts[3].y = pol.p3.z;
				}
				else {
					pts[0].x = sdPt.x;
					pts[0].y = sdPt.y;
					pts[1].x = pol.p1.x;
					pts[1].y = pol.p1.y;
					pts[2].x = pol.p2.x;
					pts[2].y = pol.p2.y;
					pts[3].x = pol.p3.x;
					pts[3].y = pol.p3.y;
				}
				a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
				b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
				c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
				bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
				if (isIn) {
					shadow = true;
					break;
				}

			}
		}
		if (shadow) {
			colRatio *= 0.25;
		}
		colRatio = pow(abs(colRatio), 0.3);
		out.x *= colRatio;
		out.y *= colRatio;
		out.z *= colRatio;
		outColors[i] = out;

	}
}
