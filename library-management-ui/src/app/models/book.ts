import { Review } from './review';

export interface Book {
  id: number;
  title: string;
  author: string;
  description: string;
  coverImage: string;
  publisher: string;
  publicationDate: Date;
  category: string;
  isbn: string;
  pageCount: number;
  isAvailable: boolean;
  averageRating: number;
  reviewCount: number;
}

export interface BookDetail extends Book {
  reviews: Review[];
  currentCheckout?: Checkout;
}

export interface Checkout {
  id: number;
  checkoutDate: Date;
  dueDate: Date;
  returnDate?: Date;
  username: string;
}