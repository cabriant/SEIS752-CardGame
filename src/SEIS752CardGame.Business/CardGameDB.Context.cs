﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SEIS752CardGame.Business
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CardGameDbEntities : DbContext
    {
        public CardGameDbEntities()
            : base("name=CardGameDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<configuration> configurations { get; set; }
        public DbSet<game> games { get; set; }
        public DbSet<player_game> player_game { get; set; }
        public DbSet<poker_table> poker_table { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<user_pwd_reset> user_pwd_reset { get; set; }
    }
}
