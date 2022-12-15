// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log2.Models
{
    internal class Parent
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Level { get; set; }

        public int Level2 { get; set; }

        public List<Son> sons { get; set; }

        public Parent Father { get; set; }

        public static Parent Demo = new Parent()
        {
            Name = "wuweilai",
            Description ="吴炜来",
            Level =12,
            Level2 =123,
            sons = new List<Son>()
            {
                new Son()
                {
                    Name ="jiangsan",
                    Description ="江三",
                    Age =27,
                    Father=new Parent()
                    {
                         Name = "wuweilai",
                         Description ="吴炜来",
                         sons = new List<Son>()
                         {
                             new Son()
                             {
                                Name ="wujiang",
                                Description ="吴江",
                                Age =27,
                             }
                         }
                    },
                    sons = new List<Son>()
            {
                new Son()
                {
                    Name ="jiangsan",
                    Description ="江三",
                    Age =27,
                    Father=new Parent()
                    {
                         Name = "wuweilai",
                         Description ="吴炜来",
                         sons = new List<Son>()
                         {
                             new Son()
                             {
                                Name ="wujiang",
                                Description ="吴江",
                                Age =27,
                             }
                         }
                    },
                    sons = new List<Son>()
                    {

                    }
                }
            }
                }
            }
        };
    }

    internal class Son
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Age { get; set; }

        public Parent Father { get; set; }

        public List<Son> sons { get; set; }
    }
}
