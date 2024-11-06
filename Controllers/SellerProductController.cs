using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Migrations;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 


namespace E_Commerce.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class SellerProductController : ControllerBase
    {

        //private readonly ISellerService _SellerService;

        //public SellerProductController(ISellerService sellerService, ECommerceDbContext context)
        //{
        //    _SellerService = sellerService;
        //}

       
    }
}
