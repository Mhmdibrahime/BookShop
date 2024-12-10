using Entities.Domains;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ShopingCart
{

    public int Id { get; set; }
    public int BookId { get; set; }

    [ForeignKey("BookId")]
    [ValidateNever]
    public Book Book { get; set; }

    [Range(1, 100, ErrorMessage = "you must enter from 1 to 100")]
    public int Count { get; set; }

    public string ApplicationUserId { get; set; }

    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public User ApplicationUser { get; set; }
    public bool IsBorrowed { get; set; }
    public IEnumerable<Book> Books { get; set; }

}