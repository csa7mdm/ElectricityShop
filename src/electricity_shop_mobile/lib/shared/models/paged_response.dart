import 'package:json_annotation/json_annotation.dart';

part 'paged_response.g.dart';

/// Generic class for paginated API responses
@JsonSerializable(genericArgumentFactories: true)
class PagedResponse<T> {
  /// Current page number
  final int pageNumber;
  
  /// Number of items per page
  final int pageSize;
  
  /// Total number of pages
  final int totalPages;
  
  /// Total number of items across all pages
  final int totalCount;
  
  /// Whether there's a previous page available
  final bool hasPrevious;
  
  /// Whether there's a next page available
  final bool hasNext;
  
  /// Collection of items for the current page
  final List<T> items;

  /// Creates a new [PagedResponse] instance
  PagedResponse({
    required this.pageNumber,
    required this.pageSize,
    required this.totalPages,
    required this.totalCount,
    required this.hasPrevious,
    required this.hasNext,
    required this.items,
  });

  /// Creates a [PagedResponse] from JSON map using a provided JSON converter for T
  factory PagedResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) => _$PagedResponseFromJson(json, fromJsonT);

  /// Converts [PagedResponse] to JSON map using a provided JSON converter for T
  Map<String, dynamic> toJson(Object? Function(T value) toJsonT) => 
      _$PagedResponseToJson(this, toJsonT);
}
