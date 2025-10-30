﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.application.Form.Dtos
{
    public class ViewFormLoaiDto
    {
        public int Id { get; set; }
        public string TenForm { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
    }
}
