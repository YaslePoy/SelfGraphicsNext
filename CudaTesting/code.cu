extern "C" {
	__constant__ double ToRad = 0.017453292519943295769236907684886;
	__constant__ double ToDeg = 57.295779513082320876798154814105;
	typedef struct {
		float3 p1;
		float3 p2;
		float3 p3;
		float3 nor;
		float3 col;
		float d;
	} PolygonCUDA;
	__global__ void func(const PolygonCUDA* data, PolygonCUDA* out) {
		int2 baseRes;
		baseRes.x = blockDim.x * gridDim.x;
		baseRes.y = blockDim.y * gridDim.y;
		int col = blockIdx.y * blockDim.y + threadIdx.y;
		int row = blockIdx.x * blockDim.x + threadIdx.x;
		int i = col + row * baseRes.x;
		PolygonCUDA pc = data[i];
		out[i] = pc;
		//double3 id;
		//id.y = col;
		//id.z = row;
		//int2 halfRes;
		//halfRes.x = baseRes.x / 2;
		//halfRes.y = baseRes.y / 2;
		//id.y -= halfRes.x;
		//id.z -= halfRes.y;
		//id.z *= -1;
		//id.y /= halfRes.x;
		//id.z /= halfRes.y;
		//double halfLenHor = sin(fow * ToRad / 2);
		//id.y *= halfLenHor;
		//id.x = sqrt(1 - pow(id.y, 2));
		//double halfLenVer = halfLenHor / baseRes.x * baseRes.y;
		//id.z *= halfLenVer;
		//double verRatio = sqrt(1 - pow(id.z, 2));
		//if (view.y != 0) {
		//	double2 temp;
		//	temp.y = verRatio * cos(view.y * ToRad) - id.z * sin(view.y * ToRad);
		//	temp.x = verRatio * sin(view.y * ToRad) + id.z * cos(view.y * ToRad);
		//	id.z = temp.x;
		//	verRatio = temp.y;
		//}
		//id.x *= verRatio;
		//id.y *= verRatio;
		//if (view.x != 0) {
		//	double2 temp;
		//	temp.x = id.x * cos(view.x * ToRad) - id.y * sin(view.x * ToRad);
		//	temp.y = id.x * sin(view.x * ToRad) + id.y * cos(view.x * ToRad);
		//	id.x = temp.x;
		//	id.y = temp.y;
		//}
		testType tt;
		tt.a = col;
		tt.b = row;
		tt.c = col * row;
		out[i] = tt;;

	}
}