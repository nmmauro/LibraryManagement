import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BookService } from '../../../services/book.service';
import { Book } from '../../../models/book';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.css'
})
export class BookFormComponent implements OnInit {
  book = {
    title: '',
    author: '',
    description: '',
    coverImage: 'https://via.placeholder.com/300x450?text=Book+Cover',
    publisher: '',
    publicationDate: new Date().toISOString().split('T')[0],
    category: '',
    isbn: '',
    pageCount: 0
  };
  
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  categories = [
    'Fiction', 'Non-Fiction', 'Science Fiction', 'Fantasy', 
    'Mystery', 'Thriller', 'Romance', 'Biography', 'History', 'Science'
  ];

  constructor(
    private bookService: BookService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    // Create a copy of the book with the proper date format
    const bookToSubmit: Partial<Book> = {
      ...this.book,
      publicationDate: new Date(this.book.publicationDate)
    };

    this.bookService.addBook(bookToSubmit).subscribe({
      next: (newBook) => {
        this.successMessage = 'Book added successfully!';
        this.isLoading = false;
        setTimeout(() => {
          this.router.navigate(['/books', newBook.id]);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage = 'Failed to add book. Please try again.';
        this.isLoading = false;
        console.error('Error adding book:', error);
      }
    });
  }
}