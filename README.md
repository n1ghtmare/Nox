Nox
===

A light, simple, flexible and generally awesome micro ORM.
The core of Nox (The Conductor) is a single class which can do a lot by itself and it is is build with extinsibility in mind.
Additionally Nox comes with a build in generic repository to help you get quickly into the action.

Simple usage
------------
You start with a DB provider, it will pick the first connection string in your config

```cs
	var provider = new SqlServerProvider();
```

Or you can specify a connection string manually

```cs
	var provider = new SqlServerProvider("ConnectionString goes here ...");
```

This is Nox's core (The Conductor) it takes as a parameter the provider to work with

```cs
	var conductor = new Conductor(provider);
```

Alright now we're ready to roll - here is some example usages:

Lets say we have an Employee entity like this

```cs
  public class Employee
  {
     public long Id { get; set; }
     public string Name { get; set; }
     public string Email { get; set; }
     public DateTime DateOfBirth { get; set; }
  }
```
           
We could execute this query and it will map for us the results as expected

```cs
	IEnumerable<Employee> results = conductor.Execute<Employee>("SELECT Id, Name, Email, DateOfBirth FROM Employee");
```

Ok, not so bad, but what about parameters ? Check this out:

```cs
	IEnumerable<Employee> results = conductor.Execute<Employee>("SELECT * FROM Employee WHERE Id = @Id", new {Id = 123});
```

Here is how you execute scalars (as an example a slightly longer query):

```cs
	decimal result = conductor.ExecuteScalar<decimal>("INSERT INTO Employee (Name, Email, DateOfBirth) VALUES (@Name, @Email, @DateOfBirth) SELECT SCOPE_IDENTITY()", 
													  new {Name = "John Doe", Email = "test@test.com", DateOfBirth = new DateTime(1970, 1, 1)});
```

You can also work with dynamic return type:

```cs
	IEnumerable<dynamic> results = conductor.Execute<dynamic>("SELECT Id, Name FROM Tasks");            
	foreach (var task in results)
	{
		Console.WriteLine("The Task Id is: {0}", task.Id);
		Console.WriteLine("The Task Name is: {0}", task.Name);
	}
```

Repository usage
----------------

Nox comes with some helpers so it can spare you some tedious work, check out the generic Repository that enables basic CRUD operations (very simple, nothing bloated):

First we create a provider-specific query composer (since every provider treats parameters, scope identity and various other things differently from one another):

```cs
	var queryComposer = new SqlServerQueryComposer();
```

Then we give it to our repository (along with the Nox core) so it knows with which database it's working

```cs
	var repository = new Repository<Employee>(conductor, queryComposer);
```

Or you can just use a provider-specific repository without passing any of those:

```cs
	var repository = new SqlServerRepository<Employee>();
```

As you can see so far, you can extend Nox and the Repository in numerous ways, anyway moving along, let's use the Repository:

```cs
	// getting all employees
	IEnumerable<Employee> allEmployees = repository.GetAll();

	// getting specific employees
	IEnumerable<Employee> specificEmployees = repository.Get("Name = @Name", new {Name = "Neo"});

	// creating an employee
	var employee = new Employee {Name = "John Doe", Email = "email", DateOfBirth = DateTime.Today};
	repository.Create(employee); // employee now has an Id if the table has identity scope

	// deleting an employee
	var employee = new Employee { Id = 123};
	repository.Delete(employee);

	// updating an employee
	var employee = new Employee { Id = 123, Name="Neo", DateOfBirth = DateTime.Today, Email = "neo@internet.com"};
	repository.Update(employee);	
```

Repository with dynamic magic
-----------------------------

The repository can be used in some interesting ways if you declare it as `dynamic`, it will try its best to interpret your queries into the correct SQL, check this out:

```cs
	// declaring it as dynamic 
	dynamic repository = new SqlServerRepository<Employee>();

	// write the query as a method
	IEnumerable<Employee> results = sqlServerRepository.GetWhere_FirstName("Neo");

	// or something a little more complex
	IEnumerable<Accounts> results = sqlServerRepository.GetWhere_FirstName_And_LastName("John", "Smith");
	IEnumerable<Accounts> results = sqlServerRepository.GetWhere_FirstName_Or_Email("John", "jsmith@internet.com");

	// classic
	IEnumerable<Accounts> results = sqlServerRepository.GetWhere_Id(1);
```

Adding more documentation is a high priority, bare with me.
More stuff is to come, contributions, suggestions and issue reports are highly appreciated.

Join me coding ! :)