# Receipt System API

## Overview

This project is a .NET API for managing a Receipt System. It allows you to perform CRUD operations on receipts, filter and sort them, and provides several statistical endpoints to analyze the data. The project is designed to be a backend for applications that need to track expenses, offering endpoints to retrieve receipts, generate reports, and analyze spending patterns.

## Features

- **CRUD Operations**: Create, read, update, and delete receipts.
- **Filtering and Sorting**: Retrieve receipts with filters such as date range, expense type, and sum range, with options for sorting.
- **Statistical Endpoints**:
	- Count receipts by expense type.
	- Calculate total sum of expenses by month.
	- Determine average sum by expense type.
	- Get the number of receipts per day of the week.
	- List the top 5 locations with the most receipts.
- **Mock Data Generation**: Generates 100 mock receipts if no data file exists.
- **Swagger Integration**: Provides interactive API documentation via Swagger UI.

## API Endpoints

### Receipts

- **GET /receipts**: Retrieve a paginated list of receipts with optional filters and sorting.
- **GET /receipts/{id}**: Retrieve a specific receipt by ID.
- **POST /receipts**: Create a new receipt.
- **PUT /receipts/{id}**: Update an existing receipt by ID.
- **DELETE /receipts/{id}**: Delete a receipt by ID.

### Statistics

- **GET /stats/count-by-expense-type**: Get the count of receipts grouped by expense type.
- **GET /stats/total-sum-by-month**: Get the total sum of receipts grouped by month.
- **GET /stats/average-sum-by-expense-type**: Get the average sum of receipts grouped by expense type.
- **GET /stats/receipts-per-day-of-week**: Get the number of receipts grouped by the day of the week.
- **GET /stats/top-locations**: Get the top 5 locations with the most receipts.

## Getting Started

### Prerequisites
- .NET SDK (version 6.0 or later)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
