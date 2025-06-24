# KnowledgeHubV2 Tests

This test project contains unit tests for the KnowledgeHubV2 Blazor WebAssembly application.

## Test Structure

The tests are organized following the same structure as the main project:

- **Models/** - Tests for data models (Note, Folder, etc.)
- **Services/** - Tests for business logic services (NoteRepository, StateContainer, FileSystemService, etc.)
- **Pages/** - Tests for Blazor components using bUnit
- **TestHelpers/** - Helper classes for setting up test scenarios

## Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio
- Open Test Explorer (Test > Test Explorer)
- Click "Run All Tests" or right-click specific tests to run

### Visual Studio Code
- Install the ".NET Core Test Explorer" extension
- Tests will appear in the Testing sidebar

## Test Technologies

- **xUnit** - Testing framework
- **bUnit** - Blazor component testing
- **Moq** - Mocking framework
- **Entity Framework InMemory** - In-memory database for repository tests

## Best Practices

1. **Naming Convention**: Tests follow the pattern `MethodName_Scenario_ExpectedResult`
2. **AAA Pattern**: Tests use Arrange-Act-Assert structure
3. **Isolation**: Each test is independent and doesn't rely on other tests
4. **Mocking**: External dependencies are mocked to ensure unit test isolation
5. **Simple & Focused**: Each test verifies one specific behavior

## Adding New Tests

When adding new features to the application:

1. Create corresponding test files in the appropriate directory
2. Write tests for:
   - Happy path scenarios
   - Edge cases
   - Error conditions
   - Validation rules
3. Ensure tests are simple, readable, and maintainable
4. Run all tests before committing changes

## Coverage Goals

While 100% coverage isn't always practical, aim to test:
- All public methods in services
- Model validation logic
- Component rendering and user interactions
- Critical business logic 