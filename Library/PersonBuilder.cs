namespace Portfolio.UserAnalytics;

public class PersonBuilder : PersonBuilder.IPersonBuilder
{
    // We use these nested public interfaces to expose the methods without allowing external instantiation
    public interface IPersonBuilder
    {
        public Person Build() => throw new NotImplementedException();
        public IPersonBuilder WithNameOf(string firstName, string surName);
        public IJobBuilder Works { get; }
        public ISpecifyAge Is { get; }
    }

    // A reference to the end product is kept so that each call can modify it. 
    private Person product { get; set; } = new Person();

    // This is the final call of the builder, and the only one that returns the actual product.
    public Person Build() => product;

    // Builder method can be on the product class or the builder itself
    public static PersonBuilder Create => new PersonBuilder();

    // This allows the PersonBuilder class to be converted to Person implicitly.
    public static implicit operator Person(PersonBuilder builder) => builder.product;

    public IPersonBuilder And()
    {
        return this;
    }

    // This is an example of a typical fluent building method
    public IPersonBuilder WithNameOf(string firstName, string lastName)
    {
        product.FirstName = firstName;
        product.LastName = lastName;
        return this;
    }

    // Job Builder shows how to create a standard facet where the methods can be called in any order
    public IJobBuilder Works => new PersonJobBuilder(product);

    // The InfoBuilder forces the fluent builder down a specific path
    public ISpecifyAge Is => new PersonInfoBuilder(product);

    /* This is a facet, or a fluent branch. We make it private and use a public interface
     so that only PersonBuilder can instantiate it. We inherit from PersonBuilder so
     that it can be used fluently with PersonBuilder. */
    private class PersonJobBuilder : PersonBuilder, IJobBuilder
    {
        internal PersonJobBuilder(Person product)
        {
            this.product = product;
        }

        public IJobBuilder AsA(string job)
        {
            product.Job = job;
            return this;
        }

        public IJobBuilder At(string company)
        {
            product.Employer = company;
            return this;
        }

        public IJobBuilder In(string state)
        {
            product.JobLocation = state;
            return this;
        }


    }

    public interface IJobBuilder : IPersonBuilder
    {
        public IJobBuilder AsA(string job);
        public IJobBuilder At(string company);
        public IJobBuilder In(string state);
    }


    // Because we want to force the fluent builder down a specific path, we do not create a 
    // single interface for the entire facet. We break it up so they can pass to each other.
    private class PersonInfoBuilder : PersonBuilder, ISpecifyAge, ISpecifyMarriageStatus
    {
        internal PersonInfoBuilder(Person product)
        {
            this.product = product;
        }

        // The methods return the interface for the next step in the chain
        public ISpecifyMarriageStatus AgeOf(int age)
        {
            if (age < 0) age = 0;
            product.Age = age;
            return this;
        }

        // The last method in the chain returns IPersonBuilder
        public IPersonBuilder MarriageStatusOf(MarriageStatus status)
        {
            if (product.Age < 18) product.MarriageStatus = MarriageStatus.NotApplicable;
            else product.MarriageStatus = status;
            return this;
        }
    }

    public interface ISpecifyAge
    {
        public ISpecifyMarriageStatus AgeOf(int age);
    }

    public interface ISpecifyMarriageStatus
    {
        public IPersonBuilder MarriageStatusOf(MarriageStatus status);
    }
}

