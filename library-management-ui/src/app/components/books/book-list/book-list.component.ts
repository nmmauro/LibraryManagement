import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../../services/book.service';
import { AuthService } from '../../../services/auth.service';
import { Book } from '../../../models/book';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './book-list.component.html',
  styleUrl: './book-list.component.css'
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  searchTerm = '';
  sortBy = 'title';
  showOnlyAvailable = false;
  isLoading = true;
  errorMessage = '';
  isLibrarian = false;

  constructor(
    private bookService: BookService,
    private authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isLibrarian = this.authService.isLibrarian;
    this.loadBooks();
  }

  loadBooks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bookService.getFeaturedBooks().subscribe({
      next: (books) => {
        this.books = books;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load books. Please try again later.';
        this.isLoading = false;
        console.error('Error loading books:', error);
      }
    });
  }

  search(): void {
    if (!this.searchTerm.trim()) {
      this.loadBooks();
      return;
    }

    this.isLoading = true;
    this.bookService.searchBooks(this.searchTerm).subscribe({
      next: (books) => {
        this.books = books;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Search failed. Please try again.';
        this.isLoading = false;
        console.error('Error searching books:', error);
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.books];
    
    // Filter by availability if needed
    if (this.showOnlyAvailable) {
      filtered = filtered.filter(book => book.isAvailable);
    }
    
    // Sort books
    filtered.sort((a, b) => {
      if (this.sortBy === 'title') {
        return a.title.localeCompare(b.title);
      } else if (this.sortBy === 'author') {
        return a.author.localeCompare(b.author);
      } else if (this.sortBy === 'rating') {
        return b.averageRating - a.averageRating;
      }
      return 0;
    });
    
    this.filteredBooks = filtered;
  }

  onFilterChange(): void {
    this.applyFilters();
  }
}