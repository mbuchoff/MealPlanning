namespace SystemOfEquations;

public record Equation(decimal X, decimal Y, decimal Z, decimal C)
{
    public static (decimal X, decimal Y, decimal Z)? Solve(Equation eq1, Equation eq2, Equation eq3)
    {
        static decimal Determinant(decimal[,] mat) =>
            mat[0, 0] * (mat[1, 1] * mat[2, 2] - mat[1, 2] * mat[2, 1]) -
            mat[0, 1] * (mat[1, 0] * mat[2, 2] - mat[1, 2] * mat[2, 0]) +
            mat[0, 2] * (mat[1, 0] * mat[2, 1] - mat[1, 1] * mat[2, 0]);

        // Coefficients of the equations
        decimal[,] A = {
            { eq1.X, eq1.Y, eq1.Z },
            { eq2.X, eq2.Y, eq2.Z },
            { eq3.X, eq3.Y, eq3.Z }
        };

        // Constants of the equations
        decimal[] B = [eq1.C, eq2.C, eq3.C];

        // Calculate the determinant of A
        decimal detA = Determinant(A);

        if (detA == 0)
        {
            return null;
        }

        // Find determinants of matrices with B replacing respective columns
        decimal[,] Ax = (decimal[,])A.Clone();
        decimal[,] Ay = (decimal[,])A.Clone();
        decimal[,] Az = (decimal[,])A.Clone();

        for (int i = 0; i < 3; i++)
        {
            Ax[i, 0] = B[i];
            Ay[i, 1] = B[i];
            Az[i, 2] = B[i];
        }

        decimal x = Determinant(Ax) / detA;
        decimal y = Determinant(Ay) / detA;
        decimal z = Determinant(Az) / detA;

        return (x, y, z);
    }
}