**CAMPUS RECRUIT POC | PET INSURANCE** 

# **POC 1 - Pet Insurance Quote Management** 

#### **Assignment** 

**Assignment window:** Complete the POC within 2 to 3 weeks. Submit the Git repository, working application, API tests, documentation, sample Excel data, and demonstration evidence. 

## **1. Business Objective** 

Build a small quote-management application that captures customer, pet, and coverage information; calculates a simplified premium; and manages the quote lifecycle from draft through conversion or expiration. 

## **Common Technical Architecture** 

Angular UI | REST/JSON ASP.NET Core Web API | Application / Business Service | Repository + EF Core + LINQ | SQLite working database | Excel import and export 

**Implementation note:** An Excel workbook should be the business data source for import/export. Use EF Core with SQLite as the working relational store so entity relationships and LINQ queries can be implemented and tested cleanly. 

### **Required Engineering Standards** 

- Angular Reactive Forms with client-side validation. 

- REST endpoints documented through Swagger/OpenAPI. 

- DTOs must be separated from EF Core entities. 

- Dependency injection, async API methods, structured logging, and global exception handling. 

- NUnit API tests using Moq and FluentAssertions; selected EF Core query tests using SQLite in-memory. 

- No plain-text secrets or passwords in source code or Excel. 

- Git feature branches, meaningful commits, pull request, and README setup steps. 

## **2. Angular UI Screens** 

### **2.1 Quote Dashboard** 

- Summary cards: total, active, expired, and converted quotes; average premium. 

- Quote grid with quote number, customer, pet, species, plan, premium, expiry date, status, and actions. 

Angular | ASP.NET Core Web API | EF Core | LINQ | Excel | NUnit | Git 

**CAMPUS RECRUIT POC | PET INSURANCE** 

- Sorting, paging, status filter, and created-date filter. 

### **2.2 Create or Edit Quote** 

- Customer fields: name, email, phone, and ZIP code. 

- Pet fields: name, species, breed, age or date of birth, gender, and pre-existing-condition indicator. 

- Coverage fields: annual limit, deductible, reimbursement percentage, and wellness option. 

- Show validation messages beside invalid fields. 

### **2.3 Quote Details and Calculation** 

- Display base premium, adjustments, discounts, final premium, expiry date, and status. 

- Actions: edit, recalculate, convert, and cancel. 

- Show a clear confirmation before conversion or cancellation. 

### **2.4 Quote Search and Reports** 

- Search by email, pet name, species, plan, status, premium range, and date range. 

- Report widgets: quotes by plan, average premium, and quotes expiring soon. 

## **3. Data Model and Excel Workbook** 

|**Sheet / Entity**|**Purpose**|**Key Fields**|
|---|---|---|
|Customers|Customer master data|CustomerId, FirstName, LastName,<br>Email, Phone, ZipCode|
|Pets|Pet information linked to customer|PetId, CustomerId, PetName, Species,<br>Breed, DateOfBirth|
|Quotes|Quote header and lifecycle|QuoteId, QuoteNumber, CustomerId,<br>PetId, CreatedDate, ExpiryDate, Status,<br>FinalPremium|
|QuoteCoverages|Selected coverage and pricing inputs|QuoteCoverageId, QuoteId, AnnualLimit,<br>Deductible, ReimbursementPct,<br>Wellness|



## **4. Core Business Logic** 

1. Customer email and pet name are mandatory. 

2. Pet age must be greater than zero and within the configured eligibility limit. 

3. Quote expiry is 30 days after creation. 

4. An expired or cancelled quote cannot be modified or converted. 

5. Final premium equals base premium plus age adjustment plus wellness amount minus eligible discounts. 

6. A quote cannot be converted more than once. 

7. A duplicate active quote for the same customer, pet, and coverage should return a validation warning. 

8. Pre-existing-condition information must be captured but does not automatically reject the quote in this POC. 

Angular | ASP.NET Core Web API | EF Core | LINQ | Excel | NUnit | Git 

**CAMPUS RECRUIT POC | PET INSURANCE** 

## **5. Required LINQ Demonstration** 

|**LINQ feature**|**Expected use**|
|---|---|
|Where|Filter active, expired, converted, and date-range results.|
|OrderByDescending|Return newest quotes first.|
|GroupBy|Summarize quote count by plan and status.|
|Average|Calculate average final premium.|
|Any|Detect duplicate active quotes.|
|Select|Project entities into dashboard and detail DTOs.|
|Skip / Take|Implement API paging.|



## **6. API Contract** 

|**Method**|**Endpoint**|**Purpose**|
|---|---|---|
|POST|/api/quotes|Create quote|
|GET|/api/quotes|Search and page quotes|
|GET|/api/quotes/{id}|Get quote details|
|PUT|/api/quotes/{id}|Update eligible quote|
|POST|/api/quotes/{id}/recalculate|Recalculate premium|
|POST|/api/quotes/{id}/convert|Convert active quote|
|DELETE|/api/quotes/{id}|Cancel quote|
|GET|/api/quotes/dashboard|Dashboard metrics|
|POST|/api/excel/quotes/import|Import workbook data|
|GET|/api/excel/quotes/export|Export workbook data|



## **7. NUnit API Testing** 

**Minimum expectation:** Implement service-level unit tests, controller tests, validation tests, and selected EF Core query tests. Include positive, negative, edge, and not-found scenarios. 

|**Test area**|**Required test scenarios**|
|---|---|
||Valid input calculates premium; zero age fails; expiry is set to|
|Quote service|30 days; expired quote cannot update; duplicate quote<br>warning is returned.|
|Conversion|Active quote converts successfully; converted or expired quote<br>fails; conversion cannot occur twice.|



Angular | ASP.NET Core Web API | EF Core | LINQ | Excel | NUnit | Git 

||**CAMPUS RECRUIT POC | PET INSURANCE**|
|---|---|
|**Test area**|**Required test scenarios**|
|Premium calculation|Age adjustment, wellness amount, and discount are applied<br>correctly; result cannot be negative.|
|Controller|Create returns 201; invalid model returns 400; missing quote<br>returns 404; valid retrieval returns 200.|
|EF Core / LINQ|Dashboard grouping, average premium, search filters, sorting,<br>and pagination return expected results using SQLite in-<br>memory.|



## **8. POC-Specific Acceptance Criteria** 

- At least four working Angular screens. 

- Premium calculation isolated in a testable service. 

- At least eight meaningful NUnit tests with clear Arrange-Act-Assert structure. 

- Excel import and export validation with friendly error messages. 

- Swagger examples, sample workbook, Postman collection, and README. 

Angular | ASP.NET Core Web API | EF Core | LINQ | Excel | NUnit | Git 

