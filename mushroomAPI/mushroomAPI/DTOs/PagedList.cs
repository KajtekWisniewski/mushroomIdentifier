﻿namespace mushroomAPI.DTOs
{
    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
