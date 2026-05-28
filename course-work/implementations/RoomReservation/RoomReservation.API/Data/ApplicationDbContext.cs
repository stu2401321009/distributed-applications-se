using Microsoft.EntityFrameworkCore;
using RoomReservation.API.Data.Entities;
using RoomReservation.API.Data.Enums;

namespace RoomReservation.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Room>()
            .Property(r => r.RoomType)
            .HasConversion<string>();

        modelBuilder.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion<string>();

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Администратор",
                LastName = "Системен",
                Email = "admin@uni.bg",
                PasswordHash = "$2a$11$W7Ayo78qgXOdNnVhxZAD4OA8secPjnSUDCpYC0kR1tGn.XZYnPZ1S",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@uni.bg",
                PasswordHash = "$2a$11$8V1JaL/fTbt6D4bGZBbFGeOSJEA1rQqEB1RoJ8byAcSfRkehaAEUe",
                Role = UserRole.Student,
                FacultyNumber = "2401321099",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 3,
                FirstName = "Мария",
                LastName = "Петрова",
                Email = "maria@uni.bg",
                PasswordHash = "$2a$11$.axHjIPkAftJ9.TU5SDNu.3vnxU5R5X0TA2HwQVnljRA3buZ7Dq.W",
                Role = UserRole.Teacher,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Аула Максима",
                Building = "Ректорат",
                Floor = 1,
                Capacity = 500,
                RoomType = RoomType.Lecture,
                HasProjector = true,
                Description = "Главната аула на университета. Използва се за тържествени събития, конференции и масови лекции.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Room
            {
                Id = 2,
                Name = "Лекционна зала 1",
                Building = "ФМИ",
                Floor = 1,
                Capacity = 80,
                RoomType = RoomType.Lecture,
                HasProjector = true,
                Description = "Просторна лекционна зала с интерактивна дъска и климатик.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Room
            {
                Id = 3,
                Name = "Компютърна лаборатория 3",
                Building = "ФМИ",
                Floor = 2,
                Capacity = 30,
                RoomType = RoomType.ComputerLab,
                HasProjector = true,
                Description = "Оборудвана с 30 работни станции. Използва се за практически упражнения по програмиране.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Room
            {
                Id = 4,
                Name = "Семинарна зала 7",
                Building = "Физически факултет",
                Floor = 1,
                Capacity = 25,
                RoomType = RoomType.Seminar,
                HasProjector = false,
                Description = "Малка зала за семинари и групови дискусии.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Room
            {
                Id = 5,
                Name = "Лекционна зала А",
                Building = "Педагогически факултет",
                Floor = 2,
                Capacity = 60,
                RoomType = RoomType.Lecture,
                HasProjector = true,
                Description = "Лекционна зала с мултимедийна система и климатик.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Room
            {
                Id = 6,
                Name = "Лаборатория по химия",
                Building = "Химически факултет",
                Floor = 1,
                Capacity = 20,
                RoomType = RoomType.Lab,
                HasProjector = false,
                Description = "Специализирана лаборатория с лабораторно оборудване.",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
