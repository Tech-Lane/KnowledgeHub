# KnowledgeHubV2

KnowledgeHubV2 is a flexible, offline-first knowledge management application built with Blazor WebAssembly. It allows users to create, organize, and link notes, embed structured data tables, and perform dynamic calculations, all while keeping their data stored securely on their local machine.

## ✨ Features

*   **Local-First Data**: All data is stored in a user-managed `.db` file on the local file system. You have full control over your data.
*   **Hierarchical Organization**: Organize your notes and documents in a familiar folder structure.
*   **Rich-Text Note Editor**: A full-featured editor for formatting notes, including headers, lists, and links.
*   **Embeddable Data Tables**: Create structured tables directly within your notes to track data.
*   **Dynamic Data References**: Pull aggregate data from your tables (like sums and averages) and display it directly in your notes with dynamic references (e.g., `{{TableName.ColumnName.sum}}`).
*   **Drag-and-Drop**: Easily reorganize your notes and folders.
*   **Backup/Restore**: Export and import your entire knowledge base to a JSON-based format (`.khb`), with optional password protection.

## 🚀 Getting Started

To get the project up and running for development or to contribute, please see the detailed instructions in the [**Maintenance and Setup Guide**](./KnowledgeHubV2/Documentation/maintenance_guide.html).

## 📚 Documentation

The project includes comprehensive documentation to help users and developers:

*   [**User Guide**](./KnowledgeHubV2/Documentation/user_guide.html): A guide for end-users on how to use the application's features.
*   [**Design Document**](./KnowledgeHubV2/Documentation/design_document.html): An overview of the application's architecture and data model.
*   [**Testing Documentation**](./KnowledgeHubV2/Documentation/testing_documentation.html): Information on the project's testing strategy and how to run tests.

## ⚖️ License

This project is licensed under the **GNU Affero General Public License v3.0**. See the [LICENSE](./LICENSE) file for the full text. 