# Creating PDF Files From Html Using Playwright.NET

Works well and works fast up to at least 10k lines. Whilst it likely remains faster than IronPDF for bigger files (40k lines took about 50s), the memory usage of the Chrome headless browser becomes a problem. At 10k lines the memory usage spikes by roughly 2GB for a few seconds and then immediately drops back, but this does not scale linearly. The following tests were run using the compiled app (in debug config):

| Number of Lines | Approximate Memory Usage Spike | Time | File Size | Notes |
|-----------------|--------------------------------|------|-----------|-------|
| 10,000 | 2GB | 5.259s | 818KB | |
| 20,000 | 4.2GB | 9.351s | 1,194KB | |
| 30,000 | 7.4GB | 15.807s | 1,760KB | |
| 40,000 | 11.9GB | 24.173s | 2,328KB | | 
| 45,000 | 14.9GB | 28.998s | 2,612KB | |
| 50,000 |  |

At 50,000 lines, Chrome crashed. This happens if you display the HTML in a normal Chrome borwser and try to print it as well. At this point, we could explore the possibility of splitting the input into smaller parts and recombining. The problem here is that this service is intended to take arbitrary HTML and render it in a PDF: This means that th service knows nothing of the content and it will split it up in a completely arbitarry fashion (maybe based on number of bytes or whatever), meaning that each chunk is no longer a valid HTML document and will not render properly.

It is worth noting that IronPDF, while more memory efficient, is noted as being CPU hungry. However, even with adequate CPU resource which isn't anywhere near fully utilised, it is very slow AND produces output files that are of the order of 7-10 times the size of those produced by Chrome when running direct comparisons with the same input files. It is unlikely to scale to bigger files any better than Playwright/Chromium does. Since we may need to produce files totalling 100,000 or more lines, we will need to think of a suitable strategy, such as giving the service paged input which it can then combine after rendering. This way the client can control the splitting of content and still guarantee that the output will make sense. It would be a compromise, but in the majority of use cases we would not need to split the input: Since this is an intended as an internal service where the consumers are known to us we can make the terms of use explicitly clear. We could even consider rejecting input that is phsyically larger than perhaps 4MB (roughly the size of the 20,000 line example file we have been using) as a bad request.
