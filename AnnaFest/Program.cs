namespace AnnaFest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(x =>
			{
				x.IdleTimeout = TimeSpan.FromSeconds(20);
				x.Cookie.IsEssential = true;
			});

			var app = builder.Build();
            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapRazorPages();

			app.Run();
		}
	}
}