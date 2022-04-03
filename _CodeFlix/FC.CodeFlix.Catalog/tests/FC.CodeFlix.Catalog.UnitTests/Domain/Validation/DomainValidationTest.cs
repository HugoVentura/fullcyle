using Bogus;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Validation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Validation
{
    public class DomainValidationTest
    {
        private Faker Faker { get; set; } = new Faker();

        [Fact(DisplayName = nameof(Test_NotNullOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void Test_NotNullOk()
        {
            var value = this.Faker.Commerce.ProductName();
            var acion = () => DomainValidation.NotNull(value, "Value");

            acion.Should().NotThrow();
        }

        [Fact(DisplayName = nameof(Test_NotNullThrowWhenNull))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void Test_NotNullThrowWhenNull()
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            string? value = null;
            Action action = () => DomainValidation.NotNull(value, fieldName);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage($"{fieldName} should not be null");
        }

        [Theory(DisplayName = nameof(Test_NotNullOrEmptyThrowWhenEmpty))]
        [Trait("Domain", "DomainValidation - Validation")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_NotNullOrEmptyThrowWhenEmpty(string? target)
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage($"{fieldName} should not be empty or null");
        }

        [Fact(DisplayName = nameof(Test_NotNullOrEmptyOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        public void Test_NotNullOrEmptyOk()
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            var target = Faker.Commerce.ProductName();
            Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

            action.Should()
                .NotThrow<EntityValidationException>();
        }

        [Theory(DisplayName = nameof(Test_MinLengthThrowWhenLess))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
        public void Test_MinLengthThrowWhenLess(string target, int minLength)
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage($"{fieldName} should be at least {minLength} characters long");
        }

        public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
        {
            yield return new object[] { "123456", 10 };
            Faker faker = new();
            for (int idx = 0; idx < numberOfTests -1; idx++)
            {
                var example = faker.Commerce.ProductName();
                var minlength = example.Length + new Random().Next(1, 20);
                yield return new object[] { example, minlength };
            }
        }

        [Theory(DisplayName = nameof(Test_MinLengthOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesGreaterThanMin), parameters: 10)]
        public void Test_MinLengthOk(string target, int minLength)
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

            action.Should().NotThrow();
        }

        public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
        {
            yield return new object[] { "123456", 6 };
            Faker faker = new();
            for (int idx = 0; idx < numberOfTests - 1; idx++)
            {
                var example = faker.Commerce.ProductName();
                var minlength = example.Length - new Random().Next(1, 5);
                yield return new object[] { example, minlength };
            }
        }

        [Theory(DisplayName = nameof(Test_MaxLengthThrowWhenGreater))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
        public void Test_MaxLengthThrowWhenGreater(string target, int maxLength)
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage($"{fieldName} should be less or equal {maxLength} characters long");
        }

        public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
        {
            yield return new object[] { "123456", 5 };
            Faker faker = new();
            for (int idx = 0; idx < numberOfTests - 1; idx++)
            {
                var example = faker.Commerce.ProductName();
                var maxlength = example.Length - new Random().Next(1, 5);
                yield return new object[] { example, maxlength };
            }
        }

        [Theory(DisplayName = nameof(Test_MaxLengthOk))]
        [Trait("Domain", "DomainValidation - Validation")]
        [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
        public void Test_MaxLengthOk(string target, int maxLength)
        {
            string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
            Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

            action.Should()
                .NotThrow();
        }

        public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
        {
            yield return new object[] { "123456", 6 };
            Faker faker = new();
            for (int idx = 0; idx < numberOfTests - 1; idx++)
            {
                var example = faker.Commerce.ProductName();
                var maxlength = example.Length + new Random().Next(0, 5);
                yield return new object[] { example, maxlength };
            }
        }
    }
}
