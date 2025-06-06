# Personal Knowledge & Data Hub

# This project is under construction!

A self-hostable web application that seamlessly blends note-taking with robust, integrated data tables and customizable dashboards. This project is being developed as the capstone for the WGU Software Engineering program as of June 2025.

![License](https://img.shields.io/badge/License-AGPL_v3-blue.svg)
![.NET Version](https://img.shields.io/badge/.NET-9.0-blueviolet)
![Framework](https://img.shields.io/badge/Framework-Blazor_WASM-blue)

## About The Project

In today's information-heavy world, individuals often struggle to manage diverse types of personal data. Existing solutions create a frustrating gap:
* **Note-taking apps** are great for unstructured thoughts but have weak table support.
* **Spreadsheet and database tools** are powerful for structured data but are too cumbersome for quick note-taking and lack a fluid, integrated writing experience.

The Personal Knowledge & Data Hub (PKDH) bridges this gap by providing a single, cohesive platform for both free-form and structured data.

### Key Features:

* **Hierarchical Note Organization:** Organize your notes and ideas within a familiar folder structure.
* **Robust, Integrated Tables:** Create powerful tables directly inside your notes. Define custom columns, specify data types (Text, Number, Date, Boolean), sort, filter, and see automatic column totals.
* **Dynamic Data Referencing:** Bring your notes to life by referencing table data directly in your text. Use simple expressions like `{{ Expenses.Amount.sum }}` to embed calculated values that update automatically.
* **Customizable Dashboards:** Create personalized dashboards to get a high-level view of your data. Add widgets to display specific notes, tables, or key statistics.
* **Self-Hostable:** Built with Docker in mind for easy deployment on your own hardware, giving you full control over your data.

### Built With

This project is built on the modern .NET ecosystem:

* [.NET 8](https://dotnet.microsoft.com/)
* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) (for the Backend Server/API)
* [Blazor WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/blazor/) (for the Frontend Client)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) (for data access)
* [SQLite](https://www.sqlite.org/index.html) (as the default database for portability)
* [.NET MAUI](https://docs.microsoft.com/en-us/dotnet/maui/) (for future desktop and mobile applications)
* [Docker](https://www.docker.com/) (for deployment)

## Getting Started

To get a local copy up and running, follow these steps.

### Prerequisites

You will need the following software installed:
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop) (Optional, for Docker-based deployment)
* A Git client (e.g., [Git SCM](https://git-scm.com/))

### Installation & Running from Source

1.  Clone the repository:
    ```sh
    git clone [https://github.com/](https://github.com/)[your-github-username]/KnowledgeHub.git
    ```
2.  Navigate to the solution directory:
    ```sh
    cd KnowledgeHub
    ```
3.  Trust the ASP.NET Core development certificate (only needs to be done once):
    ```sh
    dotnet dev-certs https --trust
    ```
4.  Run the web application from the solution's root directory:
    ```sh
    dotnet run --project KnowledgeHub.Web/KnowledgeHub.Web.csproj
    ```
5.  Open your web browser and navigate to the URL provided in the console output (e.g., `https://localhost:7123`).

### Installation & Running with Docker

1.  Clone the repository as shown above.
2.  Navigate to the solution directory.
3.  Build the Docker image. (Ensure a `Dockerfile` exists in the root directory).
    ```sh
    docker build -t knowledgehub .
    ```
4.  Run the container, mapping a local port (e.g., 8080) to the container's internal port.
    ```sh
    docker run -p 8080:8080 knowledgehub
    ```
5.  Open your web browser and navigate to `http://localhost:8080`.

## Usage

Once the application is running:
* Use the sidebar to create new notes and folders to organize your thoughts.
* Inside a note, click the "Add Table" button to create a structured data table. Give it a unique name and define your columns.
* In your note content, reference data from your tables using the `{{ TableName.ColumnName.sum }}` syntax, then click "Process Refs" to see the magic!
* Navigate to the "Dashboards" section to view your "Home" dashboard or create new ones with custom widgets to visualize your most important information.

## Roadmap

This project is currently under active development for the capstone. Future enhancements include:

* [ ] Full implementation of the .NET MAUI desktop and mobile clients.
* [ ] More advanced table features (cell formulas, linking between tables).
* [ ] An expanded set of dashboard widgets and layout customization options.
* [ ] More complex query support for data referencing.
* [ ] Official support for PostgreSQL and SQL Server databases.

See the [open issues](https://github.com/[your-github-username]/KnowledgeHub/issues) for a full list of proposed features (and known issues).

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## License

Distributed under the **GNU Affero General Public License v3.0**. See `LICENSE` for more information. This means you are free to use, modify, and distribute the code, but any modifications (including for network services) must also be open-sourced under the same license.

Project Link: [https://github.com/[your-github-username]/KnowledgeHub](https://github.com/[your-github-username]/KnowledgeHub)
