import 'package:dartz/dartz.dart';
import '../../../../core/api/api_client.dart';
import '../../../../core/config/api_config.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/product.dart';
import '../../domain/repositories/product_repository.dart';
import '../models/product_model.dart';

/// Implementation of [ProductRepository]
class ProductRepositoryImpl implements ProductRepository {
  final ApiClient _apiClient;

  ProductRepositoryImpl(this._apiClient);

  @override
  Future<Either<Failure, Pagination<Product>>> getProducts({
    required int page,
    required int pageSize,
    String? category,
    String? searchTerm,
    double? minPrice,
    double? maxPrice,
    bool? onSaleOnly,
  }) async {
    try {
      // Build query parameters
      final queryParams = <String, dynamic>{
        'pageNumber': page,
        'pageSize': pageSize,
      };

      // Add optional filters if provided
      if (category != null) {
        queryParams['categoryId'] = category;
      }
      if (searchTerm != null) {
        queryParams['searchTerm'] = searchTerm;
      }
      if (minPrice != null) {
        queryParams['minPrice'] = minPrice;
      }
      if (maxPrice != null) {
        queryParams['maxPrice'] = maxPrice;
      }
      if (onSaleOnly != null && onSaleOnly) {
        queryParams['onSaleOnly'] = true;
      }

      // Make API request
      final response = await _apiClient.get(
        ApiConfig.products,
        queryParameters: queryParams,
      );

      // Parse response into product models
      final items = (response['items'] as List)
          .map((item) => ProductModel.fromJson(item))
          .toList();

      // Create pagination object
      final pagination = Pagination<Product>(
        items: items,
        totalCount: response['totalCount'],
        pageNumber: response['pageNumber'],
        pageSize: response['pageSize'],
        totalPages: response['totalPages'],
        hasPrevious: response['hasPrevious'],
        hasNext: response['hasNext'],
      );

      return Right(pagination);
    } on ApiException catch (e) {
      return Left(_mapExceptionToFailure(e));
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred: $e',
      ));
    }
  }

  @override
  Future<Either<Failure, Product>> getProductById(String id) async {
    try {
      final response = await _apiClient.get('${ApiConfig.productById}$id');
      final product = ProductModel.fromJson(response);
      return Right(product);
    } on ApiException catch (e) {
      return Left(_mapExceptionToFailure(e));
    } catch (e) {
      return Left(UnexpectedFailure(
        message: 'An unexpected error occurred: $e',
      ));
    }
  }

  // Helper method to map API exceptions to domain failures
  Failure _mapExceptionToFailure(ApiException exception) {
    if (exception is UnauthorizedException) {
      return AuthFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is BadRequestException) {
      return ValidationFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is NotFoundException) {
      return NotFoundFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is ForbiddenException) {
      return PermissionFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is NetworkException) {
      return NetworkFailure(
        message: exception.message,
        data: exception.data,
      );
    } else if (exception is ServerException) {
      return ServerFailure(
        message: exception.message,
        data: exception.data,
      );
    } else {
      return UnexpectedFailure(
        message: exception.message,
        data: exception.data,
      );
    }
  }
}
