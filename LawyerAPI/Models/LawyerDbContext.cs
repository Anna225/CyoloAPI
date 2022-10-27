﻿using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using LawyerAPI.Models;
namespace LawyerAPI.Models
{
    public class LawyerDbContext : DbContext
    {
        public LawyerDbContext(DbContextOptions<LawyerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Court> Courts { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Lawyer> Lawyers { get; set; } = null!;
        public DbSet<CourtCaseAgenda> CourtCaseAgenda { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().Property(f => f.ID).ValueGeneratedOnAdd();            
            modelBuilder.Entity<Court>().ToTable("Courts");
            modelBuilder.Entity<CourtCaseAgenda>().ToTable("CourtCaseAgendas");
            modelBuilder.Entity<Lawyer>().ToTable("Lawyers");
            /*
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
            */
        }

    }
}
