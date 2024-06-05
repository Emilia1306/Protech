using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Protech.Models;

public partial class ProtechContext : DbContext
{
    public ProtechContext()
    {
    }

    public ProtechContext(DbContextOptions<ProtechContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BackupFile> BackupFiles { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketAdditionalTask> TicketAdditionalTasks { get; set; }

    public virtual DbSet<TicketComment> TicketComments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCategory> UserCategories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackupFile>(entity =>
        {
            entity.HasKey(e => e.IdBackupFile).HasName("PK__BackupFi__15C8BD370C52760C");

            entity.Property(e => e.IdBackupFile).HasColumnName("id_backup_file");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.BackupFiles)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK_BackupFiles_Tickets");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("PK__Tickets__48C6F52387EE03B8");

            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IdEmployee).HasColumnName("id_employee");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("priority");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("state");

            entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.TicketIdEmployeeNavigations)
                .HasForeignKey(d => d.IdEmployee)
                .HasConstraintName("FK_Tickets_Users_Employee");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.TicketIdUserNavigations)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Tickets_Users_User");
        });

        modelBuilder.Entity<TicketAdditionalTask>(entity =>
        {
            entity.HasKey(e => e.IdTicketAdditionalTask).HasName("PK__TicketAd__86576D055BD1DAAA");

            entity.Property(e => e.IdTicketAdditionalTask).HasColumnName("id_ticket_additional_task");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Finished).HasColumnName("finished");
            entity.Property(e => e.IdEmployee).HasColumnName("id_employee");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");

            entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.TicketAdditionalTasks)
                .HasForeignKey(d => d.IdEmployee)
                .HasConstraintName("FK_TicketAdditionalTasks_Users");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.TicketAdditionalTasks)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK_TicketAdditionalTasks_Tickets");
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.HasKey(e => e.IdTicketComment).HasName("PK__TicketCo__1C86B548742A38F6");

            entity.Property(e => e.IdTicketComment).HasColumnName("id_ticket_comment");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.TicketComments)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK_TicketComments_Tickets");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.TicketComments)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_TicketComments_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__D2D14637C1067BE2");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Cellphone)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cellphone");
            entity.Property(e => e.ChangePassword)
                .HasDefaultValue(true)
                .HasColumnName("change_password");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("company_name");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdUserCategory).HasColumnName("id_user_category");
            entity.Property(e => e.JobPosition)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("job_position");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");

            entity.HasOne(d => d.IdUserCategoryNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdUserCategory)
                .HasConstraintName("FK_Users_UserCategories");
        });

        modelBuilder.Entity<UserCategory>(entity =>
        {
            entity.HasKey(e => e.IdUserCategory).HasName("PK__UserCate__789E84E1D1192927");

            entity.Property(e => e.IdUserCategory).HasColumnName("id_user_category");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
