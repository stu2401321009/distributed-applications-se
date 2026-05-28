# Система за резервация на учебни зали

**Факултетен номер:** 2401321009  
**Студент:** Теодора Христова  
**Дисциплина:** Разпределени приложения

---

## Описание

Уеб система за резервация на учебни зали в Пловдивски университет „Паисий Хилендарски". Потребителите могат да разглеждат зали, да правят резервации и да следят техния статус. Администраторът управлява залите, потребителите и одобрява резервациите.

**Роли:** Student, Teacher, Admin  
**Архитектура:** REST API (ASP.NET Core) + MVC клиент (ASP.NET Core)  
**База данни:** SQL Server с Entity Framework Core (Code-First)  
**Автентикация:** JWT Bearer Token

---

## Изисквания

- .NET 9 SDK
- SQL Server (LocalDB е достатъчно)
- `dotnet-ef` инструмент: `dotnet tool install --global dotnet-ef`

---

## Стартиране

### 1. Клониране

```bash
git clone <repo-url>
cd course-work/implementations/RoomReservation
```

### 2. Конфигурация на connection string

В `RoomReservation.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RoomReservationDb;Trusted_Connection=True;"
}
```

### 3. Стартиране на API

```bash
cd RoomReservation.API
dotnet run
```

API се стартира на `http://localhost:5269`  
Swagger UI: `http://localhost:5269/swagger`

> Базата данни и seed данните се създават автоматично при първо стартиране.

### 4. Стартиране на Web клиента

В нов терминал:

```bash
cd RoomReservation.Web
dotnet run
```

Клиентът се стартира на `http://localhost:5080`

---

## Тестови акаунти

| Роля         | Email        | Парола      |
| ------------ | ------------ | ----------- |
| Admin        | admin@uni.bg | Admin1234!  |
| Студент      | ivan@uni.bg  | Student123! |
| Преподавател | maria@uni.bg | Teacher123! |
