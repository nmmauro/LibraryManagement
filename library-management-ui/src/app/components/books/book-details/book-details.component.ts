import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BookService } from '../../../services/book.service';
import { ReviewService } from '../../../services/review.service';
import { AuthService } from '../../../services/auth.service';
import { BookDetail } from '../../../models/book';
import { Review } from '../../../models/review';
import { BookFormComponent } from '../book-form/book-form.component';

@Component({
  selector: 'app-book-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, BookFormComponent],
  templateUrl: './book-details.component.html',
  styleUrl: './book-details.component.css'
})
export class BookDetailsComponent implements OnInit {
  book: BookDetail | null = null;
  isLoading = true;
  errorMessage = '';
  isLibrarian = false;
  isCustomer = false;
  newReview: Review = { rating: 5, comment: '' };
  reviewSuccess = false;
  checkoutMessage = '';
  returnMessage = '';
  showEditForm = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private reviewService: ReviewService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isLibrarian = this.authService.isLibrarian;
    this.isCustomer = this.authService.isCustomer;
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadBook(Number(id));
    }
  }

  loadBook(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.bookService.getBook(id).subscribe({
      next: (book) => {
        this.book = book;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load book details. Please try again later.';
        this.isLoading = false;
        console.error('Error loading book:', error);
      }
    });
  }

  toggleEditForm(): void {
    this.showEditForm = !this.showEditForm;
  }

  submitReview(): void {
    if (!this.book || !this.newReview.comment) return;
  
    // Add logging to help debug
    console.log('Submitting review:', this.newReview);
    
    // Ensure rating is a number, not a string (common issue with form inputs)
    const reviewToSubmit = {
      rating: Number(this.newReview.rating),
      comment: this.newReview.comment
    };
  
    this.reviewService.addReview(this.book.id, reviewToSubmit).subscribe({
      next: (response) => {
        console.log('Review submitted successfully:', response);
        this.reviewSuccess = true;
        setTimeout(() => this.reviewSuccess = false, 3000);
        // Reset the form
        this.newReview = { rating: 5, comment: '' };
        // Reload the book to get updated reviews
        this.loadBook(this.book!.id);
      },
      error: (error) => {
        console.error('Error submitting review:', error);
        // Add more helpful error handling
        if (error.status === 401) {
          this.errorMessage = 'You must be logged in to submit a review.';
        } else {
          this.errorMessage = 'Failed to submit review. Please try again.';
        }
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  checkoutBook(): void {
    if (!this.book) return;
    
    this.bookService.checkoutBook(this.book.id).subscribe({
      next: () => {
        this.checkoutMessage = 'Book checked out successfully!';
        setTimeout(() => this.checkoutMessage = '', 3000);
        this.loadBook(this.book!.id);
      },
      error: (error) => {
        this.checkoutMessage = 'Failed to checkout book. Please try again.';
        console.error('Error checking out book:', error);
      }
    });
  }

  returnBook(): void {
    if (!this.book) return;
    
    this.bookService.returnBook(this.book.id).subscribe({
      next: () => {
        this.returnMessage = 'Book returned successfully!';
        setTimeout(() => this.returnMessage = '', 3000);
        this.loadBook(this.book!.id);
      },
      error: (error) => {
        this.returnMessage = 'Failed to return book. Please try again.';
        console.error('Error returning book:', error);
      }
    });
  }

  deleteBook(): void {
    if (!this.book) return;
    
    if (confirm(`Are you sure you want to delete "${this.book.title}"?`)) {
      this.bookService.deleteBook(this.book.id).subscribe({
        next: () => {
          this.router.navigate(['/books']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete book. Please try again.';
          console.error('Error deleting book:', error);
        }
      });
    }
  }
}