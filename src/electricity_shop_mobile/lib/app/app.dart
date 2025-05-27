import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../core/api/api_client.dart';
import '../core/config/app_config.dart';
import '../core/storage/secure_storage.dart';
import '../features/auth/data/repositories/auth_repository_impl.dart';
import '../features/auth/domain/repositories/auth_repository.dart';
import '../features/auth/presentation/bloc/auth_bloc.dart';
import '../features/auth/presentation/pages/login_page.dart';
import '../features/auth/presentation/pages/register_page.dart';
import '../features/products/data/repositories/product_repository_impl.dart';
import '../features/products/domain/repositories/product_repository.dart';
import '../features/products/presentation/bloc/product_bloc.dart';
import '../features/products/presentation/pages/product_detail_page.dart';
import '../features/products/presentation/pages/product_list_page.dart';
import '../features/home/presentation/pages/home_page.dart';
import '../features/cart/data/repositories/cart_repository_impl.dart';
import '../features/cart/domain/repositories/cart_repository.dart';
import '../features/cart/presentation/bloc/cart_bloc.dart';
import '../features/cart/presentation/pages/cart_page.dart';
import 'routes.dart';

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    // Initialize services
    final secureStorage = SecureStorageService();
    final apiClient = ApiClient(secureStorage);
    // For SharedPreferences, we need to retrieve it first
    final Future<SharedPreferences> prefsProvider = SharedPreferences.getInstance();
    
    return FutureBuilder<SharedPreferences>(
      future: prefsProvider,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          final sharedPreferences = snapshot.data!;
          
          // Create repositories
          final AuthRepository authRepository = AuthRepositoryImpl(apiClient, secureStorage);
          final ProductRepository productRepository = ProductRepositoryImpl(apiClient);
          final CartRepository cartRepository = CartRepositoryImpl(sharedPreferences);
          
          return MultiBlocProvider(
            providers: [
              BlocProvider<AuthBloc>(
                create: (context) => AuthBloc(authRepository)..add(CheckAuthStatusEvent()),
              ),
              BlocProvider<ProductBloc>(
                create: (context) => ProductBloc(productRepository),
              ),
              BlocProvider<CartBloc>(
                create: (context) => CartBloc(cartRepository),
              ),
            ],
            child: MaterialApp(
              title: AppConfig.appName,
              theme: ThemeData(
                primarySwatch: Colors.blue,
                scaffoldBackgroundColor: Colors.white,
                appBarTheme: const AppBarTheme(
                  elevation: 0,
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
              ),
              initialRoute: LoginPage.routeName,
              routes: {
                LoginPage.routeName: (context) => const LoginPage(),
                RegisterPage.routeName: (context) => const RegisterPage(),
                HomePage.routeName: (context) => const HomePage(),
                ProductListPage.routeName: (context) => const ProductListPage(),
                CartPage.routeName: (context) => const CartPage(),
              },
              onGenerateRoute: (settings) {
                if (settings.name == ProductDetailPage.routeName) {
                  final productId = settings.arguments as String;
                  return MaterialPageRoute(
                    builder: (context) => ProductDetailPage(productId: productId),
                  );
                }
                return null;
              },
            ),
          );
        }
        
        // Show loading screen while SharedPreferences is being initialized
        return const MaterialApp(
          home: Scaffold(
            body: Center(
              child: CircularProgressIndicator(),
            ),
          ),
        );
      },
    );
  }
}
