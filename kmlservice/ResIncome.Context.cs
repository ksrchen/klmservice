﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kmlservice
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ResIncomeEntities : DbContext
    {
        public ResIncomeEntities()
            : base("name=ResIncomeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ResIncome> ResIncomes { get; set; }
        public virtual DbSet<Favoriate> Favoriates { get; set; }
        public virtual DbSet<attachment> attachments { get; set; }
        public virtual DbSet<vwResIncomeSummary> vwResIncomeSummaries { get; set; }
        public virtual DbSet<vwExpense> vwExpenses { get; set; }
        public virtual DbSet<vwFavoriate> vwFavoriates { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
    }
}
