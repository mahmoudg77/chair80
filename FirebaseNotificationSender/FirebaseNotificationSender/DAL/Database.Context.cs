﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FirebaseNotificationSender.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class chari80_dbEntities : DbContext
    {
        public chari80_dbEntities()
            : base("name=chari80_dbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sec_sessions> sec_sessions { get; set; }
        public virtual DbSet<sec_users> sec_users { get; set; }
        public virtual DbSet<tbl_accounts> tbl_accounts { get; set; }
        public virtual DbSet<trip_book> trip_book { get; set; }
        public virtual DbSet<trip_request> trip_request { get; set; }
        public virtual DbSet<trip_request_details> trip_request_details { get; set; }
        public virtual DbSet<trip_share> trip_share { get; set; }
        public virtual DbSet<trip_share_details> trip_share_details { get; set; }
        public virtual DbSet<vwTripsDetail> vwTripsDetails { get; set; }
        public virtual DbSet<vwTripSeatDetail> vwTripSeatDetails { get; set; }
    }
}