﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.shared.Interfaces
{
    public interface IFullAudited : ICreatedBy, IModifiedBy, ISoftDeleted
    {
    }

    public interface ISoftDeleted
    {
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }

    public interface ICreatedBy
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public interface IModifiedBy
    {
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
