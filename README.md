# Supabase Minimal API

This is a minimal API using the C# Supabase Client. The project was built as a proof of concept for a mob-programming project and allows for operations on a selected Supabase Table, with the addition of the requisite Supabase Url and API key within the program.cs where noted.

## The Models

The table in question contained a model for the table items as:
- Id - GUID (primary key within the table)
- Url - String
- JobText - String
- CreatedAt - DateTime

The progam also includes contracts for the DTOs based on requests and responses relating to the above model.

## The Routes

All routes are based on the /jobdescriptions endpoints.  The following operations are contained within the API:

- GET - /jobdescriptions/ - returns a list of all job descriptions.
- GET - /jobdescriptions/{id} - returns the job description identified.
- POST - /jobdescriptions - adds a job description based on the request and returns the job description id generated by the table.
- DELETE - /jobdescriptions/{id} - deletes the selected job description.

## Next Steps

~~A GetAll route is being worked on, though due to the nature of the client, a custom deserialiser will also need to be added to allow the response to be parsed.~~

NB - The get all route is now working, with a workaround for the serialization issue. 
