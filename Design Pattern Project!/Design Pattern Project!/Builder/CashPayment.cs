﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsProject.Builder
{
    public class CashPayment : IPayment
    {
        public void ProcessPayment()
        {
            Console.WriteLine("Processing cash payment...");
        }
    }
}
