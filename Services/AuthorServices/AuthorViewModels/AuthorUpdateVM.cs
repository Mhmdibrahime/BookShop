﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthorServices.AuthorViewModels
{
    public class AuthorUpdateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Email { get; set; }
    }
}
