using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Dtos.Key;

public class PaginatedKeys
{
	public ICollection<KeyDto> Keys { get; set; } = new List<KeyDto>();
	public int TotalCount { get; set; }
	public int TotalPages { get; set; } = 0;
	public int CurrentPage { get; set; } = 1;
	public int PageSize { get; set; }

}

