using System.Collections.Generic;

namespace project_AI
{
    internal enum TitleMatchMode
    {
        Like,      
        Exact,      
        StartsWith,  
        EndsWith     
    }

    internal class QueryIntent
    {
        public bool WantBooks { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public int? Id { get; set; }
        public bool IdNot { get; set; }
        public int? IdGreaterThan { get; set; }
        public int? IdLessThan { get; set; }
        public int? IdFrom { get; set; }
        public int? IdTo { get; set; }
        public bool WantListIds { get; set; }
        public string? Title { get; set; }                 
        public List<string>? TitleValues { get; set; }      
        public TitleMatchMode? TitleMode { get; set; }
        public bool TitleNot { get; set; }
        public bool WantListTitles { get; set; }
        public string? TitleJoiner { get; set; }           
        public string? Author { get; set; }                 
        public List<string>? AuthorValues { get; set; }     
        public bool AuthorNot { get; set; }
        public bool WantListAuthors { get; set; }
        public string? AuthorJoiner { get; set; }          
        public string? Category { get; set; }              
        public List<string>? CategoryValues { get; set; }   
        public bool CategoryNot { get; set; }
        public bool WantListCategories { get; set; }
        public string? CategoryJoiner { get; set; }        
        public int? YearEq { get; set; }
        public bool YearNot { get; set; }
        public int? YearAfter { get; set; }
        public int? YearBefore { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public bool WantListYears { get; set; }
        public List<int>? YearValues { get; set; }
        public string? YearJoiner { get; set; }             
        public int? Limit { get; set; }
    }
}
