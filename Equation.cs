namespace SystemOfEquations;

public record Equation(double X, double Y, double Z, double C)
{
    public static (double X, double Y, double Z) Solve(Equation eq1, Equation eq2, Equation eq3)
    {
        static double Determinant(double[,] mat)
        {
            return mat[0, 0] * (mat[1, 1] * mat[2, 2] - mat[1, 2] * mat[2, 1]) -
                   mat[0, 1] * (mat[1, 0] * mat[2, 2] - mat[1, 2] * mat[2, 0]) +
                   mat[0, 2] * (mat[1, 0] * mat[2, 1] - mat[1, 1] * mat[2, 0]);
        }

        // Coefficients of the equations
        double[,] A = {
            { eq1.X, eq1.Y, eq1.Z },
            { eq2.X, eq2.Y, eq2.Z },
            { eq3.X, eq3.Y, eq3.Z }
        };

        // Constants of the equations
        double[] B = { eq1.C, eq2.C, eq3.C };

        // Calculate the determinant of A
        double detA = Determinant(A);

        if (detA == 0)
        {
            return (double.NaN, double.NaN, double.NaN);
        }

        // Find determinants of matrices with B replacing respective columns
        double[,] Ax = (double[,])A.Clone();
        double[,] Ay = (double[,])A.Clone();
        double[,] Az = (double[,])A.Clone();

        for (int i = 0; i < 3; i++)
        {
            Ax[i, 0] = B[i];
            Ay[i, 1] = B[i];
            Az[i, 2] = B[i];
        }

        double x = Determinant(Ax) / detA;
        double y = Determinant(Ay) / detA;
        double z = Determinant(Az) / detA;

        return (x, y, z);
    }
}