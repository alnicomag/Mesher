using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODEsolver
{
    delegate double SystemEq(double t, double[] x);

    class RungeKutta4th
    {
        public RungeKutta4th()
        {

        }
        public RungeKutta4th(double delta, double timespan, long eqnum)
        {
            this.delta = delta;
            stepnum = (long)Math.Ceiling(timespan / delta) + 1;
            this.eqnum = eqnum;

            x = new double[this.eqnum, stepnum];
            time = new double[stepnum];
            for (long i = 0; i < stepnum; ++i)
            {
                time[i] = i * this.delta;
            }
        }

        public void RegistrateEq(SystemEq CompEq)
        {
            Eq.Add(CompEq);
        }

        public void ode45(params double[] initials)
        {
            if (Eq.Count != eqnum)
            {
                throw new InvalidOperationException("");
            }

            for (int i = 0; i < eqnum; ++i)
            {
                x[i, 0] = initials[i];
            }

            double[] k1 = new double[eqnum];
            double[] k2 = new double[eqnum];
            double[] k3 = new double[eqnum];
            double[] k4 = new double[eqnum];
            double[] temp_x = new double[eqnum];

            for (long i = 0; i < (stepnum - 1); ++i)
            {
                for (int j = 0; j < eqnum; j++)
                    temp_x[j] = x[j, i];
                for (int j = 0; j < eqnum; j++)
                    k1[j] = delta * Eq[j](time[i], temp_x);

                for (int j = 0; j < eqnum; j++)
                    temp_x[j] = x[j, i] + k1[j] * 0.5;
                for (int j = 0; j < eqnum; j++)
                    k2[j] = delta * Eq[j](time[i] + delta * 0.5, temp_x);

                for (int j = 0; j < eqnum; j++)
                    temp_x[j] = x[j, i] + k2[j] * 0.5;
                for (int j = 0; j < eqnum; j++)
                    k3[j] = delta * Eq[j](time[i] + delta * 0.5, temp_x);

                for (int j = 0; j < eqnum; j++)
                    temp_x[j] = x[j, i] + k3[j];
                for (int j = 0; j < eqnum; j++)
                    k4[j] = delta * Eq[j](time[i] + delta, temp_x);

                for (int j = 0; j < eqnum; j++)
                    x[j, i + 1] = x[j, i] + (k1[j] + 2.0 * (k2[j] + k3[j]) + k4[j]) / 6;
            }
        }

        private long stepnum;        //時間節点の数
        private double delta;       //時間刻み[s]
        private long eqnum;          //独立変数の数
        private double[,] x;         //解
        private double[] time;
        private List<SystemEq> Eq = new List<SystemEq>();
    }
}
