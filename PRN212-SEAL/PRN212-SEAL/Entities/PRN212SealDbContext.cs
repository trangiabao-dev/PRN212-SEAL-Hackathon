using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN212_SEAL.Entities;

public partial class PRN212SealDbContext : DbContext
{
    public PRN212SealDbContext(DbContextOptions<PRN212SealDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Challenge> Challenges { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC07BBFA3FD0");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ_Account_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Account_Username").IsUnique();

            entity.HasIndex(e => e.StudentCode, "UX_Account_StudentCode")
                .IsUnique()
                .HasFilter("([StudentCode] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentCode)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Challeng__3214EC07295A88F9");

            entity.ToTable("Challenge");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC075DC8B468");

            entity.ToTable("Score");

            entity.HasIndex(e => new { e.SubmissionId, e.JudgeId }, "UQ_Score_Submission_Judge").IsUnique();

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.ScoreValue).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ScoredAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Judge).WithMany(p => p.Scores)
                .HasForeignKey(d => d.JudgeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_Account_Judge");

            entity.HasOne(d => d.Submission).WithMany(p => p.Scores)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_Submission");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Submissi__3214EC07EE4808D9");

            entity.ToTable("Submission");

            entity.HasIndex(e => new { e.TeamId, e.ChallengeId }, "UQ_Submission_Team_Challenge").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.GithubUrl).HasMaxLength(500);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Challenge).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submission_Challenge");

            entity.HasOne(d => d.Team).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submission_Team");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Team__3214EC07D918809F");

            entity.ToTable("Team");

            entity.HasIndex(e => e.LeaderId, "UQ_Team_LeaderId").IsUnique();

            entity.HasIndex(e => e.TeamName, "UQ_Team_TeamName").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TeamName).HasMaxLength(100);

            entity.HasOne(d => d.Leader).WithOne(p => p.Team)
                .HasForeignKey<Team>(d => d.LeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_Account_Leader");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeamMemb__3214EC0787FF9B40");

            entity.ToTable("TeamMember");

            entity.HasIndex(e => e.StudentCode, "UQ_TeamMember_StudentCode").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.StudentCode)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeamMember_Team");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
