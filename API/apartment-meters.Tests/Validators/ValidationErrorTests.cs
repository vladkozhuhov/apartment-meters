using Application.Models.MeterReadingModel;
using Application.Models.UsersModel;
using Application.Models.WaterMeterModel;
using Application.Validators;
using Domain.Enums;
using FluentValidation.TestHelper;

namespace Tests.Validators;

/// <summary>
/// Тесты для валидаторов моделей
/// </summary>
public class ValidationErrorTests
{
    #region MeterReadingValidation

    /// <summary>
    /// Тесты валидатора для добавления показаний счетчика
    /// </summary>
    public class MeterReadingAddDtoValidatorTests
    {
        private readonly MeterReadingAddDtoValidator _validator = new();

        /// <summary>
        /// Проверяет, что возникает ошибка при пустом идентификаторе счетчика
        /// </summary>
        [Fact]
        public void Should_HaveError_When_WaterMeterId_IsEmpty()
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.Empty,
                WaterValue = "12345,678",
                ReadingDate = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WaterMeterId)
                .WithErrorCode(ErrorType.EmptyWaterMeterIdError470.ToString());
        }

        /// <summary>
        /// Проверяет, что возникает ошибка при невалидном идентификаторе счетчика
        /// </summary>
        [Fact]
        public void Should_HaveError_When_WaterMeterId_IsInvalid()
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                WaterValue = "12345,678",
                ReadingDate = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WaterMeterId)
                .WithErrorCode(ErrorType.EmptyWaterMeterIdError470.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных значений показаний счетчика
        /// </summary>
        [Theory]
        [InlineData("", ErrorType.EmptyWaterValueError480)]
        [InlineData(null, ErrorType.EmptyWaterValueError480)]
        [InlineData("123", ErrorType.InvalidWaterValueFormatError481)]
        [InlineData("123.456", ErrorType.InvalidWaterValueFormatError481)]
        [InlineData("12345678", ErrorType.InvalidWaterValueFormatError481)]
        [InlineData("123456,789", ErrorType.InvalidWaterValueFormatError481)]
        public void Should_HaveError_When_WaterValue_IsInvalid(string waterValue, ErrorType expectedErrorType)
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.NewGuid(),
                WaterValue = waterValue,
                ReadingDate = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WaterValue)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных дат показаний
        /// </summary>
        [Theory]
        [InlineData(null, ErrorType.EmptyReadingDateError482)]
        public void Should_HaveError_When_ReadingDate_IsInvalid(DateTime? readingDate, ErrorType expectedErrorType)
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.NewGuid(),
                WaterValue = "12345,678",
                ReadingDate = readingDate ?? default
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ReadingDate)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет, что возникает ошибка при будущей дате показаний
        /// </summary>
        [Fact]
        public void Should_HaveError_When_ReadingDate_IsInFuture()
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.NewGuid(),
                WaterValue = "12345,678",
                ReadingDate = DateTime.UtcNow.AddYears(1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ReadingDate)
                .WithErrorCode(ErrorType.FutureReadingDateError483.ToString());
        }

        /// <summary>
        /// Проверяет, что валидация проходит успешно для корректной модели
        /// </summary>
        [Fact]
        public void Should_NotHaveError_When_Model_IsValid()
        {
            var model = new MeterReadingAddDto
            {
                WaterMeterId = Guid.NewGuid(),
                WaterValue = "12345,678",
                ReadingDate = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region UserValidation

    /// <summary>
    /// Тесты валидатора для добавления пользователя
    /// </summary>
    public class UserAddDtoValidatorTests
    {
        private readonly UserAddDtoValidator _validator = new();

        /// <summary>
        /// Проверяет различные случаи невалидных номеров квартир
        /// </summary>
        [Theory]
        [InlineData(0, ErrorType.InvalidApartmentNumberError451)]
        [InlineData(-1, ErrorType.InvalidApartmentNumberError451)]
        public void Should_HaveError_When_ApartmentNumber_IsInvalid(int apartmentNumber, ErrorType expectedErrorType)
        {
            var model = CreateValidUserAddDto();
            model.ApartmentNumber = apartmentNumber;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ApartmentNumber)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных фамилий
        /// </summary>
        [Theory]
        [InlineData("", ErrorType.EmptyLastNameError452)]
        [InlineData(null, ErrorType.EmptyLastNameError452)]
        [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor", ErrorType.LastNameTooLongError453)]
        public void Should_HaveError_When_LastName_IsInvalid(string lastName, ErrorType expectedErrorType)
        {
            var model = CreateValidUserAddDto();
            model.LastName = lastName;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных имен
        /// </summary>
        [Theory]
        [InlineData("", ErrorType.EmptyFirstNameError454)]
        [InlineData(null, ErrorType.EmptyFirstNameError454)]
        [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor", ErrorType.FirstNameTooLongError455)]
        public void Should_HaveError_When_FirstName_IsInvalid(string firstName, ErrorType expectedErrorType)
        {
            var model = CreateValidUserAddDto();
            model.FirstName = firstName;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет случай слишком длинного отчества
        /// </summary>
        [Theory]
        [InlineData("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor", ErrorType.MiddleNameTooLongError456)]
        public void Should_HaveError_When_MiddleName_IsTooLong(string middleName, ErrorType expectedErrorType)
        {
            var model = CreateValidUserAddDto();
            model.MiddleName = middleName;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MiddleName)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных паролей
        /// </summary>
        [Theory]
        [InlineData("", ErrorType.EmptyPasswordError457)]
        [InlineData(null, ErrorType.EmptyPasswordError457)]
        [InlineData("pass", ErrorType.PasswordTooShortError458)]
        [InlineData("password", ErrorType.InvalidPasswordFormatError459)]
        [InlineData("PASSWORD123", ErrorType.InvalidPasswordFormatError459)]
        [InlineData("Password", ErrorType.InvalidPasswordFormatError459)]
        public void Should_HaveError_When_Password_IsInvalid(string password, ErrorType expectedErrorType)
        {
            var model = CreateValidUserAddDto();
            model.Password = password;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет, что валидация проходит успешно для корректной модели
        /// </summary>
        [Fact]
        public void Should_NotHaveError_When_Model_IsValid()
        {
            var model = CreateValidUserAddDto();

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private static UserAddDto CreateValidUserAddDto()
        {
            return new UserAddDto
            {
                ApartmentNumber = 101,
                LastName = "Иванов",
                FirstName = "Иван",
                MiddleName = "Иванович",
                Password = "Password123",
                PhoneNumber = "+71234567890",
                Role = UserRole.User
            };
        }
    }

    #endregion

    #region WaterMeterValidation

    /// <summary>
    /// Тесты валидатора для добавления счетчика
    /// </summary>
    public class WaterMeterAddDtoValidatorTests
    {
        private readonly WaterMeterAddDtoValidator _validator = new();

        /// <summary>
        /// Проверяет, что возникает ошибка при пустом идентификаторе пользователя
        /// </summary>
        [Fact]
        public void Should_HaveError_When_UserId_IsEmpty()
        {
            var model = CreateValidWaterMeterAddDto();
            model.UserId = Guid.Empty;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                .WithErrorCode(ErrorType.EmptyUserIdError463.ToString());
        }

        /// <summary>
        /// Проверяет различные случаи невалидных заводских номеров
        /// </summary>
        [Theory]
        [InlineData("", ErrorType.EmptyFactoryNumberError473)]
        [InlineData(null, ErrorType.EmptyFactoryNumberError473)]
        [InlineData("12345678901", ErrorType.FactoryNumberTooLongError474)]
        [InlineData("123-456-78", ErrorType.InvalidFactoryNumberFormatError475)]
        public void Should_HaveError_When_FactoryNumber_IsInvalid(string factoryNumber, ErrorType expectedErrorType)
        {
            var model = CreateValidWaterMeterAddDto();
            model.FactoryNumber = factoryNumber;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FactoryNumber)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет случай будущей даты установки счетчика
        /// </summary>
        [Theory]
        [InlineData("2100-01-01", ErrorType.FutureFactoryYearError477)]
        public void Should_HaveError_When_FactoryYear_IsInFuture(string factoryYearStr, ErrorType expectedErrorType)
        {
            DateOnly factoryYear = DateOnly.Parse(factoryYearStr);

            var model = CreateValidWaterMeterAddDto();
            model.FactoryYear = factoryYear;

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FactoryYear)
                .WithErrorCode(expectedErrorType.ToString());
        }

        /// <summary>
        /// Проверяет, что валидация проходит успешно для корректной модели
        /// </summary>
        [Fact]
        public void Should_NotHaveError_When_Model_IsValid()
        {
            var model = CreateValidWaterMeterAddDto();

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private static WaterMeterAddDto CreateValidWaterMeterAddDto()
        {
            return new WaterMeterAddDto
            {
                UserId = Guid.NewGuid(),
                PlaceOfWaterMeter = PlaceOfWaterMeter.Bathroom,
                WaterType = WaterType.Cold,
                FactoryNumber = "ABC123456",
                FactoryYear = DateOnly.FromDateTime(DateTime.Now.AddYears(-1))
            };
        }
    }

    #endregion
} 