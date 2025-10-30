﻿using AutoMapper;
using syll.be.application.Auth.Dtos.User;
using syll.be.application.DanhBa.Dtos;
using syll.be.application.Form.Dtos;
using syll.be.application.ToChuc.Dtos;
using syll.be.domain.Auth;
using syll.be.domain.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace syll.be.application.Base
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, ViewUserDto>();
            CreateMap<AppUser, ViewMeDto>();
            CreateMap<domain.ToChuc.ToChuc, ViewToChucDto>();
            //CreateMap<domain.DanhBa.DanhBa, ViewDanhBaDto>();
            CreateMap<FormLoai, ViewFormLoaiDto>();
            CreateMap<FormDauMuc, ViewFormDauMucDto>();
            CreateMap<FormLoai, ViewFormDauMucDto>();
        } 
        }
}
