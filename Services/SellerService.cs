using E_Commerce.Context;
using E_Commerce.DTO;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce.Services
{
    public class SellerService/* : ISellerService*/
    {
        ECommerceDbContext _context;

        public SellerService(ECommerceDbContext context)
        {
            _context = context;
        }

        
    }
}
