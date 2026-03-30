using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Dtos;
using WorkerService.Models;

namespace WorkerService.MappingProfile
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderTemp, OrderTempWorkerDto>()
                .ForMember(dest => dest.DetailCount, opt => opt.Ignore());
        }
    }
}
