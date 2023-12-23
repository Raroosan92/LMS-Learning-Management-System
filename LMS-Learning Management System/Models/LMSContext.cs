using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LMS_Learning_Management_System.Models
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardSubject> CardSubjects { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<TeacherEnrollment> TeacherEnrollments { get; set; }
        public virtual DbSet<ActiveSession> ActiveSessions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=LMS;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");


            modelBuilder.Entity<ActiveSession>(entity =>
            {
                entity.ToTable("Active_Sessions");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LoginDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Login_Date");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("User_ID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(256)
                    .HasColumnName("User_Name");


                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(256)
                    .HasColumnName("PhoneNumber");

                entity.Property(e => e.DeviceType)
                    .HasMaxLength(256)
                    .HasColumnName("DeviceType");
            });
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CardNo).HasColumnName("Card_No");

                entity.Property(e => e.CardPassword)
                    .IsRequired()
                    .HasColumnName("Card_Password");

                entity.Property(e => e.CardPrice).HasColumnName("Card_Price");

                entity.Property(e => e.CardStatus).HasColumnName("Card_Status");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("User_ID");

                entity.Property(e => e.UserName).HasColumnName("User_Name");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cards_AspNetUsers");
            });

            modelBuilder.Entity<CardSubject>(entity =>
            {
                entity.ToTable("Card_Subjects");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CardNo).HasColumnName("Card_No");

                entity.Property(e => e.ClassId).HasColumnName("Class_ID");

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.HasOne(d => d.CardNoNavigation)
                    .WithMany(p => p.CardSubjects)
                    .HasForeignKey(d => d.CardNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Card_Subjects_Cards");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.CardSubjects)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Card_Subjects_Classes");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.CardSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Card_Subjects_Subjects");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreatedUser).HasColumnName("Created_User");

                entity.Property(e => e.Descriptions).IsRequired();
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClassId).HasColumnName("Class_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("User_Id");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollment_Classes");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollment_Subjects");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollment_AspNetUsers");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClassId).HasColumnName("Class_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreatedUser).HasColumnName("Created_User");

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.Property(e => e.UrlVideo).HasColumnName("URL_Video");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lessons_Classes");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lessons_Subjects");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Abbreviation).IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreatedUser).HasColumnName("Created_User");
            });

            modelBuilder.Entity<TeacherEnrollment>(entity =>
            {
                entity.ToTable("Teacher_Enrollments");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClassId).HasColumnName("Class_ID");

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("User_ID");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
