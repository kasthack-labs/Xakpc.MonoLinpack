using System.Diagnostics;

namespace Xakpc.MonoLinpack.Core
{
	public class Linpack
	{
		double abs(double d)
		{
			return (d >= 0) ? d : -d;
		}

		static T[][] CreateArray<T>(int rows, int cols)
		{
			var array = new T[rows][];
			for (int i = 0; i < array.GetLength(0); i++)
				array[i] = new T[cols];

			return array;
		}

		// problem size = psize x psize
		private const int DEFAULT_PSIZE = 500;
		double _mflopsResult;
		double _residnResult;
		string _timeResult;
		double _epsResult;

		public void RunBenchmark()
		{
		    int i;

		    const int n = DEFAULT_PSIZE;
			const int lda = DEFAULT_PSIZE+1;
			const int ldaa = DEFAULT_PSIZE;
            const double ops = (2.0e0 * (n * n * n)) / 3.0 + 2.0 * (n * n);

			var a = CreateArray<double>(ldaa, lda);

			var b = new double[ldaa];
			var x = new double[ldaa];
		    var ipvt = new int[ldaa];

            
			Matgen(a, n, b);

			var sw = new Stopwatch();
			sw.Reset();
			sw.Start();

			this.dgefa(a, lda, n, ipvt);
			this.Dgesl(a, n, ipvt, b, 0);
			
            sw.Stop();

			var total = sw.ElapsedMilliseconds / 1000.0;
            for (i = 0; i < n; i++)
		        x[ i ] = b[ i ];
		    var norma = Matgen(a, n, b);
		    for (i = 0; i < n; i++)
		        b[ i ] = -b[ i ];
		    dmxpy(n, b, n, lda, x, a);
			var resid = 0.0;
			var normx = 0.0;
			for (i = 0; i < n; i++)
			{
				resid = (resid > abs(b[i])) ? resid : abs(b[i]);
				normx = (normx > abs(x[i])) ? normx : abs(x[i]);
			}

		  
			this._epsResult = epslon(1.0);
			this._residnResult = resid / (n * norma * normx * this._epsResult);
			this._residnResult += 0.005;
			this._residnResult = (int)(this._residnResult * 100);
			this._residnResult /= 100;

			this._timeResult = sw.Elapsed.ToString();

			this._mflopsResult = ops / (1.0e6 * total);
			this._mflopsResult += 0.0005; // for rounding
			this._mflopsResult = (int)(this._mflopsResult * 1000);
			this._mflopsResult /= 1000;

            //Debug.WriteLine("Mflops/s: " + this._mflopsResult +
            //    "  Time: " + this._timeResult +
            //    "  Norm Res: " + this._residnResult +
            //    "  Precision: " + this._epsResult);
		}

		public double MFlops
		{
			get { return this._mflopsResult; }
		}

		public double ResIDN
		{
			get { return this._residnResult; }
		}

		public string Time
		{
			get { return this._timeResult; }
		}

		public double Eps
		{
			get { return this._epsResult; }
		}

	    static double Matgen(double[][] a, int n, double[] b)
		{
		    int i, j;

			var init = 1325;
			var norma = 0.0;
			/*  Next two for() statements switched.  Solver wants
			matrix in column order. --dmd 3/3/97
			*/
			for (i = 0; i < n; i++)
			{
				for (j = 0; j < n; j++)
				{
					init = 3125 * init % 65536;
					a[j][i] = (init - 32768.0) / 16384.0;
					norma = (a[j][i] > norma) ? a[j][i] : norma;
				}
			}
			for (i = 0; i < n; i++)
			{
				b[i] = 0.0;
			}
			for (j = 0; j < n; j++)
			{
				for (i = 0; i < n; i++)
				{
					b[i] += a[j][i];
				}
			}

			return norma;
		}
		int dgefa(double[][] a, int lda, int n, int[] ipvt)
		{
			double[] col_k, col_j;
			double t;
			int j, k, kp1, l, nm1;
			int info;

			// gaussian elimination with partial pivoting

			info = 0;
			nm1 = n - 1;
			if (nm1 >= 0)
			{
				for (k = 0; k < nm1; k++)
				{
					col_k = a[k];
					kp1 = k + 1;

					// find l = pivot index

					l = idamax(n - k, col_k, k, 1) + k;
					ipvt[k] = l;

					// zero pivot implies this column already triangularized

					if (col_k[l] != 0)
					{

						// interchange if necessary

						if (l != k)
						{
							t = col_k[l];
							col_k[l] = col_k[k];
							col_k[k] = t;
						}

						// compute multipliers

						t = -1.0 / col_k[k];
						dscal(n - (kp1), t, col_k, kp1, 1);

						// row elimination with column indexing

						for (j = kp1; j < n; j++)
						{
							col_j = a[j];
							t = col_j[l];
							if (l != k)
							{
								col_j[l] = col_j[k];
								col_j[k] = t;
							}
							daxpy(n - (kp1), t, col_k, kp1, 1,
							  col_j, kp1, 1);
						}
					}
					else
					{
						info = k;
					}
				}
			}
			ipvt[n - 1] = n - 1;
			if (a[(n - 1)][(n - 1)] == 0) info = n - 1;

			return info;
		}
		void Dgesl(double[][] a, int n, int[] ipvt, double[] b, int job)
		{
			double t;
			int k, kb, l,
			    kp1;

			int nm1 = n - 1;
			if (job == 0)
			{
				if (nm1 >= 1)
				{
					for (k = 0; k < nm1; k++)
					{
						l = ipvt[k];
						t = b[l];
						if (l != k)
						{
							b[l] = b[k];
							b[k] = t;
						}
						kp1 = k + 1;
						daxpy(n - (kp1), t, a[k], kp1, 1, b, kp1, 1);
					}
				}
				for (kb = 0; kb < n; kb++)
				{
					k = n - (kb + 1);
					b[k] /= a[k][k];
					t = -b[k];
					daxpy(k, t, a[k], 0, 1, b, 0, 1);
				}
			}
			else
			{
				for (k = 0; k < n; k++)
				{
					t = ddot(k, a[k], 0, 1, b, 0, 1);
					b[k] = (b[k] - t) / a[k][k];
				}
				if (nm1 >= 1)
				{
					for (kb = 1; kb < nm1; kb++)
					{
						k = n - (kb + 1);
						kp1 = k + 1;
						b[k] += ddot(n - (kp1), a[k], kp1, 1, b, kp1, 1);
						l = ipvt[k];
						if (l != k)
						{
							t = b[l];
							b[l] = b[k];
							b[k] = t;
						}
					}
				}
			}
		}
		unsafe void daxpy(int n, double da, double[] dx, int dx_off, int incx,
				double[] dy, int dy_off, int incy)
		{
			int i, ix, iy;
		    if ( ( n <= 0 ) || ( da == 0 ) ) return;
		    if ( incx == 1 && incy == 1 ){
                fixed (double* dyp = dy) {
                    fixed ( double* dxp = dx ) {
                        var dypw = dyp + dy_off;
                        var dxpw = dxp + dx_off;
                        for (i = 0; i < n; i++)
                            *(dypw++) += da * *(dxpw++);
                    }
                }
		        return;
		    }
		    ix = 0;
		    iy = 0;
		    if ( incx < 0 ) ix = ( -n + 1 ) * incx;
		    if ( incy < 0 ) iy = ( -n + 1 ) * incy;
		    for (i = 0; i < n; i++) {
		        dy[ iy + dy_off ] += da * dx[ ix + dx_off ];
		        ix += incx;
		        iy += incy;
		    }
		}
		double ddot(int n, double[] dx, int dx_off, int incx, double[] dy,
				 int dy_off, int incy)
		{
			double dtemp;
			int i, ix, iy;

			dtemp = 0;

			if (n > 0)
			{

				if (incx != 1 || incy != 1)
				{
					ix = 0;
					iy = 0;
					if (incx < 0) ix = (-n + 1) * incx;
					if (incy < 0) iy = (-n + 1) * incy;
					for (i = 0; i < n; i++)
					{
						dtemp += dx[ix + dx_off] * dy[iy + dy_off];
						ix += incx;
						iy += incy;
					}
				}
				else
				{
					for (i = 0; i < n; i++)
						dtemp += dx[i + dx_off] * dy[i + dy_off];
				}
			}
			return (dtemp);
		}
		void dscal(int n, double da, double[] dx, int dx_off, int incx)
		{
			int i, nincx;

			if (n > 0)
			{
				if (incx != 1)
				{
					nincx = n * incx;
					for (i = 0; i < nincx; i += incx)
						dx[i + dx_off] *= da;
				}
				else
				{
					for (i = 0; i < n; i++)
						dx[i + dx_off] *= da;
				}
			}
		}
		int idamax(int n, double[] dx, int dx_off, int incx)
		{
			double dmax, dtemp;
			int i, ix, itemp = 0;

			if (n < 1)
			{
				itemp = -1;
			}
			else if (n == 1)
			{
				itemp = 0;
			}
			else if (incx != 1)
			{

				// code for increment not equal to 1

				dmax = abs(dx[0 + dx_off]);
				ix = 1 + incx;
				for (i = 1; i < n; i++)
				{
					dtemp = abs(dx[ix + dx_off]);
					if (dtemp > dmax)
					{
						itemp = i;
						dmax = dtemp;
					}
					ix += incx;
				}
			}
			else
			{

				// code for increment equal to 1

				itemp = 0;
				dmax = abs(dx[0 + dx_off]);
				for (i = 1; i < n; i++)
				{
					dtemp = abs(dx[i + dx_off]);
					if (dtemp > dmax)
					{
						itemp = i;
						dmax = dtemp;
					}
				}
			}
			return (itemp);
		}
		double epslon(double x)
		{
			double a, b, c, eps;

			a = 4.0e0 / 3.0e0;
			eps = 0;
			while (eps == 0)
			{
				b = a - 1.0;
				c = b + b + b;
				eps = abs(c - 1.0);
			}
			return (eps * abs(x));
		}
		void dmxpy(int n1, double[] y, int n2, int ldm, double[] x, double[][] m)
		{
			int j, i;

			// cleanup odd vector
			for (j = 0; j < n2; j++)
			{
				for (i = 0; i < n1; i++)
				{
					y[i] += x[j] * m[j][i];
				}
			}
		}
	}
}
