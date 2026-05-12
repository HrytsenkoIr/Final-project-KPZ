ReminderNotebook

ReminderNotebook — це десктопний додаток для Windows (WPF), призначений для створення нотаток, керування категоріями та налаштування нагадувань (одноразових та періодичних). Проєкт розроблений з дотриманням принципів чистого коду, патернів проєктування та архітектури MVVM.

Функціонал

- Керування нотатками: Створення, редагування, видалення та архівація нотаток.
- Категорії: Створення категорій з кастомними кольорами для візуальної організації.
- Нагадування:
  - Встановлення дати та часу.
  - Одноразові та повторювані нагадування (щодня, щотижня, щомісяця).
  - Спливаючі сповіщення (Pop-up) при спрацюванні.
- Пошук та фільтрація: Швидкий пошук за текстом та фільтрація нотаток за категоріями.
- Автоматичне збереження: Використання бази даних SQLite для надійного зберігання даних.

Запуск проєкту локально

Вимоги
- .NET 8.0 SDK
- Windows (для роботи WPF)

Інструкція
Клонуйте репозиторій.
2. Відкрийте термінал у кореневій папці проєкту.
3. Виконайте команду для запуску:
   powershell
   dotnet run --project ReminderNotebook/ReminderNotebook.csproj
   
Або відкрийте файл `ReminderNotebook.sln` у Visual Studio / JetBrains Rider та натисніть F5.



Programming Principles

У проєкті дотримано наступні принципи програмування:

1.  SRP (Single Responsibility Principle): Кожен клас має одну зону відповідальності. Наприклад, [CategoryService.cs](ReminderNotebook/Services/CategoryService.cs) відповідає лише за бізнес-логіку категорій, а [CategoryRepository.cs](ReminderNotebook/Repositories/CategoryRepository.cs) — за взаємодію з БД.
2.  DIP (Dependency Inversion Principle): Високорівневі модулі (ViewModels) залежать від абстракцій (інтерфейсів), а не від конкретних реалізацій. Див. [MainViewModel.cs](ReminderNotebook/ViewModels/MainViewModel.cs).
3.  DRY (Don't Repeat Yourself): Спільна логіка винесена у базові класи ([BaseViewModel.cs](ReminderNotebook/ViewModels/BaseViewModel.cs)) та допоміжні методи ([RelayCommand.cs](ReminderNotebook/Helpers/RelayCommand.cs)).
4.  SoC (Separation of Concerns): Чіткий поділ на рівні представлення (Views), логіки (ViewModels), бізнес-правил (Services) та даних (Models).
5.  KISS (Keep It Simple, Stupid): Використання простої та надійної бібліотеки `Microsoft.Data.Sqlite` для роботи з даними без надмірних абстракцій ORM.


Design Patterns

Проєкт використовує наступні патерни проєктування:

1.  Repository Pattern: Використовується для ізоляції логіки доступу до даних від бізнес-логіки.
    - [ICategoryRepository.cs](ReminderNotebook/Repositories/Interfaces/ICategoryRepository.cs)
    - [CategoryRepository.cs](ReminderNotebook/Repositories/CategoryRepository.cs)
2.  Observer Pattern: Реалізований для системи сповіщень. `NotificationService` підписується на події `ReminderNotifier`.
    - [IReminderObserver.cs](ReminderNotebook/Observers/IReminderObserver.cs)
    - [ReminderNotifier.cs](ReminderNotebook/Observers/ReminderNotifier.cs)
3.  Factory Pattern: Використовується для централізованого створення складних об'єктів.
    - [NoteFactory.cs](ReminderNotebook/Factories/NoteFactory.cs)
    - [ReminderFactory.cs](ReminderNotebook/Factories/ReminderFactory.cs)
4.  MVVM (Model-View-ViewModel): Основний архітектурний патерн проєкту, що забезпечує незалежність UI від логіки.
    - Models: [Note.cs](ReminderNotebook/Models/Note.cs)
    - Views: [MainWindow.xaml](ReminderNotebook/Views/MainWindow.xaml)
    - ViewModels: [MainViewModel.cs](ReminderNotebook/ViewModels/MainViewModel.cs)


Refactoring Techniques

Під час розробки та рефакторингу були застосовані такі техніки:

1.  Extract Class: Логіку взаємодії з БД винесено з ViewModels у окремі Repositories, а складні розрахунки — у Services.
2.  Encapsulate Field: Використання властивостей (Properties) замість публічних полів для контролю доступу та виклику подій оновлення UI. Див. [ReminderViewModel.cs](ReminderNotebook/ViewModels/ReminderViewModel.cs).
3.  Rename Method/Variable: Перейменування методів (наприклад, з `DoAction` на `ProcessPendingReminders`) для кращого розуміння коду без коментарів.
4.  Decompose Conditional: Складні умови у валідації були розбиті на окремі методи для покращення читабельності. Див. [ReminderEditWindow.xaml.cs](ReminderNotebook/Views/ReminderEditWindow.xaml.cs).
5.  Remove Dead Code: Видалення застарілих обробників подій та невикористаних UI-елементів (наприклад, видалення кнопки "Save" у категоріях на користь автозбереження).


Статистика коду

Загальна кількість рядків C# коду (без урахування розмітки XAML, конфігів та стилів): >2000 рядків.
Команда для перевірки: `Get-ChildItem -Recurse -Filter *.cs | Get-Content | Measure-Object -Line`
