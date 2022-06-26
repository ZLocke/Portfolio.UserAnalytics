namespace Portfolio.UserAnalytics;

public class RandomUserGenerator
{
    private readonly TelemetryClient _logger;
    public RandomUserGenerator(TelemetryConfiguration telemetryConfiguration)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
    }

    [FunctionName("RandomUserGenerator")]
    public void Run([TimerTrigger("0 */60 * * * *")] TimerInfo myTimer)
    {
        string firstName = RandomPersonData.GetRandomFirstName();
        string lastName = RandomPersonData.GetRandomLastName();
        string state = RandomPersonData.GetRandomState();
        string job = RandomPersonData.GetRandomJob();
        MarriageStatus marriageStatus = RandomPersonData.GetRandomMarriageStatus();
        string company = RandomPersonData.GetRandomCompany();
        int age = RandomPersonData.GetRandomAge();

        Person person = PersonBuilder.Create
            .WithNameOf(firstName, lastName)
            .Works.At(company).In(state).AsA(job)
            .Is.AgeOf(age).MarriageStatusOf(marriageStatus)
            .Build();

        Dictionary<string, string> properties = new Dictionary<string, string>()
        {
            {"FirstName", person.FirstName},
            {"LastName", person.LastName},
            {"Age", person.Age.ToString() }     ,
            {"Marriage Status", person.MarriageStatus.ToString() },
            {"Job", person.Job },
            {"Employer", person.Employer },
            {"Location", person.JobLocation }
        };

        // We log the event.
        _logger.TrackEvent("RandomUserGenerator called", properties);
    }
}
