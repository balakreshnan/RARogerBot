﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocksecbot
{
    // Defines a state property used to track information about the user.
    public class UserProfile
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Date { get; set; }
    }
}
