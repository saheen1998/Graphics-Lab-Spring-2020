using UnityEngine;
using System;
using System.Collections;

public class Func : MonoBehaviour {

	//Distance between two points
	public static double DistTo(Vector3 p1, Vector3 p2) {
		
		var X = Math.Pow(p1.x - p2.x, 2);
		var Y = Math.Pow(p1.y - p2.y, 2);
		var Z = Math.Pow(p1.z - p2.z, 2);
		return Math.Sqrt(X + Y + Z);
	}

	//Shortest angle difference
	public static float AngleDiff(float alpha, float beta) {
        float phi = Math.Abs(beta - alpha) % 360;
        float diff = phi > 180 ? 360 - phi : phi;
        return diff;
    }

	//Dot Product of 2 matrices
	public static double DotProduct(double[,] m1, double[,] m2) {
		double dot = 0;
		for(int i = 0; i<3; i++)
			for(int j = 0; j<3 ; j++)
				dot += m1[i,j] * m2[i,j];
		return dot;
 	}

	//Matrix multiplication
	public static double[,] matMul(double[,] a, double[,] b, int m, int n, int p, int q){
		
        double[,] c = new double[m, q];
		if(n != p) {
            LogHandler.Logger.Log("Func.cs: Matrix multiplication not possible", LogType.Error);
         } else {
            for (int i = 0; i < m; i++)
               for (int j = 0; j < q; j++) {
                  c[i, j] = 0;
                  for (int k = 0; k < n; k++)
                     c[i, j] += a[i, k] * b[k, j];
               }
		 }
		 return c;
	}

	//Exponent map
	public static double[,] exp_map(double[,] w){

		double theta;

		theta = Math.Sqrt( w[0,0]*w[0,0] + w[1,0]*w[1,0] + w[2,0]*w[2,0]) + Math.Pow(10,-30);

		for(int i = 0; i<3; i++)
			w[i,0] = w[i,0]/theta;
			
		double[,] w_hat = {{0, -w[2,0], w[1,0]}, {w[2,0], 0, -w[0,0]}, {-w[1,0], w[0,0], 0}};
		
		double[,] res = {{1,0,0} , {0,1,0} , {0,0,1}};
		double[,] dot = matMul(w_hat,w_hat, 3, 3, 3, 3);

		for(int i = 0; i<3; i++)
			for(int j = 0; j<3 ; j++)
				res[i,j] += w_hat[i,j] * Math.Sin(theta) + dot[i,j] * (1 - Math.Cos(theta));

		return res;
	}

	//Check constraint value for plane
	public static double check_constraint_plane(double wx, double wy, double wz, double dx, double dy, double dz, double[,] norm, double[] p){

		double[,] w = {{-wx},{wz},{-wy}};	//Transpose matrix of [wx wy wz]
		double[,] md = {{dx}, {dz}, {dy}};	//Transpose matrix of [dx dy dz]
		double[,] ew = new double[3, 3];
		ew = exp_map(w);

		double[,] term1 = new double[3,1];	//ew * [dx dy dz]T
		double[,] term2 = new double[3,1];	//ew * norm

		//term1 becomes = [ew * [dx dy dz]T - P]
		term1 = matMul(ew, md, 3, 3, 3, 1);
		term1[0,0]-=p[0];
		term1[1,0]-=p[1];
		term1[2,0]-=p[2];

		//Transpose of term1 = [ew * [dx dy dz]T - P]T
		double[,] term1_T = {{term1[0,0], term1[1,0], term1[2,0]}};

		term2 = matMul(ew, norm, 3, 3, 3, 1);

		//[ew * [dx dy dz]T - P]T * (ew * [0 0 1]T)
		double[,] res = matMul(term1_T, term2, 1, 3, 3, 1);
		return res[0,0];
	}

	//Check constraint value for relaxed axial
	public static int check_constraint_axial(double wx, double wy, double wz, double dx, double dy, double dz, double rad, double[] p){

		double[,] w = {{-wx},{wz},{-wy}};	//Transpose matrix of [wx wy wz]
		double[,] md = {{dx}, {dz}, {dy}};	//Transpose matrix of [dx dy dz]
		double[,] norm = {{0}, {1}, {0}};	//Transpose matrix of [0 0 1]
		double[,] ew = new double[3, 3];
		ew = exp_map(w);

		double[,] term1 = new double[3,1];	//[dx dy dz]T
		double[,] term2 = new double[3,1];	//ew * [0 0 1]T

		double[,] c2 = new double[3,1];

		//term1 becomes = [P - [dx dy dz]T]
		for(int i = 0; i<3; i++)
			term1[i,0] = p[i] - md[i,0];

		c2 = term1;

		//Transpose of term1 = [P - [dx dy dz]T]T
		double[,] term1_T = {{term1[0,0], term1[1,0], term1[2,0]}};

		term2 = matMul(ew, norm, 3, 3, 3, 1);

		//[ew * P - [dx dy dz]T]T * (ew * [0 0 1]T)
		double[,] res1 = matMul(term1_T, term2, 1, 3, 3, 1);

		//Check constraint2 for circle
		double len = Math.Sqrt(c2[0,0]*c2[0,0] + c2[1,0]*c2[1,0] + c2[2,0]*c2[2,0]);
		double res2 = len*len - rad*rad;

		//0 if not in constraint, 1 if in constraint
		if(res1[0,0] < 0.001 && res1[0,0] > -0.001 && res2 < 0.001 && res2 > -0.001)
			return 1;
		else
			return 0;
	}

	//Matrix to quaternion
	public static void matToQ(double[,] ew, ref float qx, ref float qy, ref float qz, ref float qw){
		float tr = (float)(ew[0,0] + ew[1,1] + ew[2,2]);

		if (tr > 0) { 
			float S = (float)Math.Sqrt(tr+1.0) * 2; // S=4*qw 
			qw = 0.25f * S;
			qx = (float)(ew[2,1] - ew[1,2]) / S;
			qy = (float)(ew[0,2] - ew[2,0]) / S; 
			qz = (float)(ew[1,0] - ew[0,1]) / S; 
		} else if ((ew[0,0] > ew[1,1]) && (ew[0,0] > ew[2,2])) { 
			float S = (float)Math.Sqrt(1.0 + ew[0,0] - ew[1,1] - ew[2,2]) * 2; // S=4*qx 
			qw = (float)(ew[2,1] - ew[1,2]) / S;
			qx = 0.25f * S;
			qy = (float)(ew[0,1] + ew[1,0]) / S; 
			qz = (float)(ew[0,2] + ew[2,0]) / S; 
		} else if (ew[1,1] > ew[2,2]) { 
			float S = (float)Math.Sqrt(1.0 + ew[1,1] - ew[0,0] - ew[2,2]) * 2; // S=4*qy
			qw = (float)(ew[0,2] - ew[2,0]) / S;
			qx = (float)(ew[0,1] + ew[1,0]) / S; 
			qy = 0.25f * S;
			qz = (float)(ew[1,2] + ew[2,1]) / S; 
		} else { 
			float S = (float)Math.Sqrt(1.0 + ew[2,2] - ew[0,0] - ew[1,1]) * 2; // S=4*qz
			qw = (float)(ew[1,0] - ew[0,1]) / S;
			qx = (float)(ew[0,2] + ew[2,0]) / S;
			qy = (float)(ew[1,2] + ew[2,1]) / S;
			qz = 0.25f * S;
		}
	}
}
