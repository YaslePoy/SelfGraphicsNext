extern "C" {
	__constant__ int CountOfTrns = 512;
	__constant__ float3 trPoint1[512];
	__constant__ float3 trPoint2[512];
	__constant__ float3 trPoint3[512];
	__constant__ float3 normals[512];
	__constant__ float dRatios[512];
	__constant__ float3 colors[512];
	__global__ void resultPixel(const float3* rays, const float3 xyz,const int count, float3* outColors) {
		int i = blockDim.x * blockIdx.x + threadIdx.x;
		if (i < count) {
			float3 mpl = rays[i];
			float3 baseColor;
			baseColor.x = 25;
			baseColor.y = 25;
			baseColor.z = 25;
			float3 red;
			red.x = 255;
			float3 green;
			green.y = 255;
			float3 blue;
			blue.z = 255;
			//outColors[i] = baseColor;
			float minDist;
			minDist = 512000000;
			float3 out;
			//out = base
			for (int j = 0; j < CountOfTrns; j++)
			{
				float3 abc = normals[j];
				float upper = dRatios[j] + abc.x * xyz.x + abc.y * xyz.y + abc.z * xyz.z;
				float lower = abc.x * mpl.x + abc.y * mpl.y + abc.z * mpl.z;
				if (lower == 0 && upper == 0) {
				}
				else if (upper > 0 && lower == 0) {
				}
				else {
					float t = -(upper / lower);
					if (t < 0) {
					}
					else {
						float3 newPoint;
						newPoint.x = mpl.x * t;
						newPoint.y = mpl.y * t;
						newPoint.z = mpl.z * t;
						float newLen = sqrt(pow(newPoint.x, 2) + pow(newPoint.y, 2) + pow(newPoint.z, 2));
						bool around = newLen < minDist;
						if (around) {
							newPoint.x += xyz.x;
							newPoint.y += xyz.y;
							newPoint.z += xyz.z;
							float2 pts[4];
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
								minDist = newLen;
								out = red;
							}
							else {
								out = green;
							}
						}
					}
				}
			}
			outColors[i] = out;
		}
	}
}