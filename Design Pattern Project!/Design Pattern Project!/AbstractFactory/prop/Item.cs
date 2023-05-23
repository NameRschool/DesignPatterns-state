﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternsProject.AbstractFactory.prop
{
    public abstract class Item
    {
        public string Name { get; set; }
        public virtual double Price { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $" {Name} מחיר- {Price:C2}";
            
        }
    }
}
