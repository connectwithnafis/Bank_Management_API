# Bank Management App

Welcome to the **Bank Management App** repository! This project is a full-stack banking simulation system that allows customers, bank employees, and admins to manage accounts, perform transactions, apply for loans, and view reports. It includes both a backend API and a frontend interface to handle various banking operations securely.

## Key Features

### 1. **User Authentication and Authorization**
- **Roles**: Customers, bank employees, and administrators.
- **JWT Authentication**: Secure API access using JWT tokens for user authentication and authorization.

### 2. **Account Management**
- **Account Types**: Support for checking, savings, and loan accounts.
- **CRUD Operations**: Create, read, update, and delete accounts.
- **Account Details**: View balance, account history, and customer information.

### 3. **Transactions and Fund Transfers**
- **Deposit and Withdrawals**: Enable customers to deposit and withdraw funds.
- **Fund Transfers**: Perform internal and external fund transfers between different banks.
- **Transaction Limits and Fees**: Set daily transaction limits and apply fees for specific account types.
- **Transaction History**: Store and retrieve transaction history for each account.

### 4. **Loan Management**
- **Loan Application**: Customers can apply for loans with fields for loan type, amount, and term.
- **Loan Approval Workflow**: Admins or managers can review, approve, or reject loan applications.

### 5. **Admin Dashboard**
- **User and Account Management**: Admins can manage user accounts, reset passwords, and control account status.

## Tech Stack

- **Frontend**: 
  - React
  - React Router
  - Axios (for making API requests)

## Installation

### 1. **Clone the repository**

```bash
git clone https://github.com/connectwithnafis/Bank_Management_App.git
cd Bank_Management_App
```
- Install dependencies:

```bash
npm install
```

- Run the development server:

```bash
npm start
```

This will start the React frontend at `http://localhost:3000`.
