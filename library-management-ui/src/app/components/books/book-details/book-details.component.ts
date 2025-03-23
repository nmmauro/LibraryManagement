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

    this.reviewService.addReview(this.book.id, this.newReview).subscribe({
      next: () => {
        this.reviewSuccess = true;
        setTimeout(() => this.reviewSuccess = false, 3000);
        this.newReview = { rating: 5, comment: '' };
        this.loadBook(this.book!.id);
      },
      error: (error) => {
        console.error('Error submitting review:', error);
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