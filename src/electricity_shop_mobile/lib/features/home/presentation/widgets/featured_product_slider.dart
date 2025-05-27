import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';

class FeaturedProductSlider extends StatefulWidget {
  const FeaturedProductSlider({super.key});

  @override
  State<FeaturedProductSlider> createState() => _FeaturedProductSliderState();
}

class _FeaturedProductSliderState extends State<FeaturedProductSlider> {
  final PageController _pageController = PageController();
  int _currentPage = 0;

  // Dummy data for featured products
  final List<Map<String, dynamic>> _featuredProducts = [
    {
      'title': 'Smart LED Lighting',
      'subtitle': 'Energy efficient solutions for your home',
      'image': 'https://via.placeholder.com/600x300/4D78D0/FFFFFF?text=Smart+Lighting',
    },
    {
      'title': 'Professional Tools',
      'subtitle': 'Quality tools for every job',
      'image': 'https://via.placeholder.com/600x300/D04D4D/FFFFFF?text=Pro+Tools',
    },
    {
      'title': 'Home Automation',
      'subtitle': 'Make your home smarter',
      'image': 'https://via.placeholder.com/600x300/4DD067/FFFFFF?text=Home+Automation',
    },
  ];

  @override
  void initState() {
    super.initState();
    _startAutoScroll();
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  void _startAutoScroll() {
    Future.delayed(const Duration(seconds: 5), () {
      if (mounted) {
        final nextPage = (_currentPage + 1) % _featuredProducts.length;
        _pageController.animateToPage(
          nextPage,
          duration: const Duration(milliseconds: 500),
          curve: Curves.easeInOut,
        );
        _startAutoScroll();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(
          height: 200,
          child: PageView.builder(
            controller: _pageController,
            itemCount: _featuredProducts.length,
            onPageChanged: (index) {
              setState(() {
                _currentPage = index;
              });
            },
            itemBuilder: (context, index) {
              final product = _featuredProducts[index];
              return Container(
                margin: const EdgeInsets.all(10),
                child: Stack(
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(10),
                      child: CachedNetworkImage(
                        imageUrl: product['image'],
                        fit: BoxFit.cover,
                        width: double.infinity,
                        placeholder: (context, url) => Container(
                          color: Colors.grey[300],
                        ),
                        errorWidget: (context, url, error) => Container(
                          color: Colors.grey[300],
                          child: const Icon(Icons.error),
                        ),
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      left: 0,
                      right: 0,
                      child: Container(
                        padding: const EdgeInsets.all(15),
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.bottomCenter,
                            end: Alignment.topCenter,
                            colors: [
                              Colors.black.withOpacity(0.7),
                              Colors.transparent,
                            ],
                          ),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              product['title'],
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            Text(
                              product['subtitle'],
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 14,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              );
            },
          ),
        ),
        // Page indicator
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(
            _featuredProducts.length,
            (index) => Container(
              width: 8,
              height: 8,
              margin: const EdgeInsets.symmetric(horizontal: 4),
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: _currentPage == index
                    ? Theme.of(context).primaryColor
                    : Colors.grey[300],
              ),
            ),
          ),
        ),
        const SizedBox(height: 16),
      ],
    );
  }
}
