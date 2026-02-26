using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scaffoldong.Models;

namespace Scaffoldong.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext (DbContextOptions<EmployeeContext> options)
            : base(options)
        {
        }

        public DbSet<Scaffoldong.Models.Employee> Employee { get; set; } = default!;
        public DbSet<Scaffoldong.Models.BorrowEquipment1> BorrowEquipment1 { get; set; } = default!;
        public DbSet<Scaffoldong.Models.tMember> tMember { get; set; } = default!;

        public DbSet<Scaffoldong.Models.tManager> tManager { get; set; } =default!;

        public DbSet<Scaffoldong.Models.Application_Details> Application_Details { get; set; } = default!;

        public DbSet<Scaffoldong.Models.Application> Application { get; set; } = default!;
        public DbSet<Scaffoldong.Models.Equipment_Details> Equipment_Details { get; set; } = default!;

        public DbSet<Scaffoldong.Models.Equipment> Equipment { get; set; } = default!;
        public DbSet<Scaffoldong.Models.Application_Completed> Application_Completed { get; set; } = default!;
        
          public DbSet<Scaffoldong.Models.IsSendEmail_OverdueNotification> IsSendEmail_OverdueNotification { get; set; } = default!;
        public DbSet<Scaffoldong.Models.Count_Borrowing_Times> Count_Borrowing_Times { get; set; } = default!;

        public DbSet<Scaffoldong.Models.Count_Borrowing_Times_BySelectDate> Count_Borrowing_Times_BySelectDate { get; set; } = default!;
        //建立種子資料
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "David", Mobile = "0933-152667", Email = "david@gmail.com", Department = "總經理室", Title = "CEO" },
                new Employee { Id = 2, Name = "Mary", Mobile = "0938-456889", Email = "mary@gmail.com", Department = "人事部", Title = "管理師" },
                new Employee { Id = 3, Name = "Joe", Mobile = "0925-331225", Email = "joe@gmail.com", Department = "財務部", Title = "經理" },
                new Employee { Id = 4, Name = "Mark", Mobile = "0935-863991", Email = "mark@gmail.com", Department = "業務部", Title = "業務員" },
                new Employee { Id = 5, Name = "Rose", Mobile = "0987-335668", Email = "rose@gmail.com", Department = "資訊部", Title = "工程師" },
                new Employee { Id = 6, Name = "May", Mobile = "0955-259885", Email = "may@gmail.com", Department = "資訊部", Title = "工程師" },
                new Employee { Id = 7, Name = "John", Mobile = "0921-123456", Email = "john@gmail.com", Department = "業務部", Title = "業務員" }
               );
        }
    }
}
